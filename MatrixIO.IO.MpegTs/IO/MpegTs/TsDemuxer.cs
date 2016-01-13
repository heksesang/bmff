using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MatrixIO.IO.MpegTs.Streams;
using MatrixIO.IO.MpegTs.Tables;

namespace MatrixIO.IO.MpegTs
{
    public class TsDemuxer
    {
        private readonly object _syncObject = new object();

        public IList<TsProgram> Programs { get; private set; }

        private readonly Dictionary<ushort, TsStream> _streams = new Dictionary<ushort, TsStream>();

        public ICollection<TsStream> Streams
        {
            get { return _streams.Values; }
        }

        private int _partialPacketLength = 0;
        private readonly byte[] _partialPacket = new byte[TsPacket.Length];

        public TsDemuxer()
        {
            Programs = Portability.CreateList<TsProgram>();

            var programAssociationStream = new TableStream()
                                               {PacketIdentifier = (ushort) PacketIdentifier.ProgramAssociationTable};
            programAssociationStream.UnitReceived += ProgramAssociationTableReceived;
            _streams.Add((ushort) PacketIdentifier.ProgramAssociationTable, programAssociationStream);

            var descriptionStream = new TableStream() {PacketIdentifier = (ushort) PacketIdentifier.TsDescriptionTable};
            descriptionStream.UnitReceived += DescriptionTableReceived;
            _streams.Add((ushort) PacketIdentifier.TsDescriptionTable, descriptionStream);
        }

        public void ProcessInput(byte[] buffer, int offset = 0)
        {
            ProcessInput(buffer, offset, buffer.Length);
        }


        public void ProcessInput(byte[] buffer, int offset, int length)
        {
            int position = offset;
            int remainder = 0;

            if (_partialPacketLength != 0)
            {
                Debug.WriteLine("Using previous " + _partialPacketLength + " byte partial packet.");
                remainder = TsPacket.Length - _partialPacketLength;
                int len = Math.Min(remainder, length);
                Debug.WriteLine("Copying " + len + " additional bytes to partial packet.");
                Buffer.BlockCopy(buffer, 0, _partialPacket, _partialPacketLength, len);
                _partialPacketLength += len;
                position += len;

                Debug.WriteLine("Partial packet is now " + _partialPacketLength + " bytes.");
                if (_partialPacketLength >= TsPacket.Length)
                {
                    _partialPacketLength = 0;
                    try
                    {
                        var packet = new TsPacket(_partialPacket);
                        ProcessPacket(packet);
                        Debug.WriteLine("Reassembled partial packet.");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }
                }
            }

            while ((offset + length - position) >= TsPacket.Length)
            {
                if (buffer[position] == 0x47)
                {
                    try
                    {
                        var packet = new TsPacket(buffer, position);
                        position += TsPacket.Length;
                        Debug.WriteLine("Processing packet.");
                        ProcessPacket(packet);
                    }
                    catch
                    {
                        Debug.WriteLine("Error deserializing packet.  Assuming false start code and scanning ahead for the next packet.");
                        position++;
                    }
                }
                else
                {
                    Debug.WriteLine("Skipping byte 0x" + buffer[position].ToString("X2") + " at offset " + position + ".");
                    position++;
                }
            }

            remainder = offset + length - position;
            if (remainder > 0)
            {
                Debug.WriteLine("Remainder is " + remainder + " bytes.");
                int packetStartOffset = -1;
                for (int i = 0; i < remainder; i++)
                {
                    if (buffer[position + i] == 0x47)
                    {
                        packetStartOffset = i;
                        break;
                    }
                    Debug.WriteLine("Skipping byte 0x" + buffer[position].ToString("X2") + " at offset " + (position + i) + ".");
                }
                if (packetStartOffset >= 0)
                {
                    Debug.WriteLine("Found sync byte in remainder.  Storing partial packet.");
                    _partialPacketLength = remainder - packetStartOffset;
                    Buffer.BlockCopy(buffer, position + packetStartOffset, _partialPacket, 0, _partialPacketLength);
                }
                else _partialPacketLength = 0;
            }
        }

        public void ProcessPacket(TsPacket packet)
        {
            if (packet.PacketIdentifier == (ushort) PacketIdentifier.NullPacket)
                return; // null packet for padding strict muxrate streams
#if DEBUG2
            var dbg = new StringBuilder();
            dbg.Append("Received packet for PID " + (PacketIdentifier) packet.PacketIdentifier + ". ");
            dbg.Append("Continuity is " + packet.ContinuityCounter + ". ");
            if (packet.AdaptationField != null) dbg.Append("Has Adaptation Field. ");
            if (packet.Payload != null) dbg.Append("Has " + packet.Payload.Length + " byte Payload. ");
            if (packet.PayloadUnitStartIndicator) dbg.Append("First in series. ");
            Debug.WriteLine(dbg.ToString());
#endif
            TsStream stream;
            if (_streams.TryGetValue(packet.PacketIdentifier, out stream)) stream.ProcessInput(packet);

        }

        private void ProgramAssociationTableReceived(object sender, TsStreamEventArgs eventArgs)
        {
            var e = eventArgs as TsStreamEventArgs<TsTable>;
            if (e == null) return;

            lock (_syncObject)
            {


                var pat = e.DecodedUnit.TableIdentifier == TableIdentifier.ProgramAssociation
                              ? (ProgramAssociationTable) e.DecodedUnit
                              : null;

                if (pat == null) return;

#if DEBUG2
            Debug.WriteLine("Table is a " + pat.TableIdentifier + " with Identifier 0x" + pat.Identifier.ToString("X4") +
                            " and IsCurrent=" + pat.IsCurrent);
            foreach (var pa in pat.Rows)
                Debug.WriteLine("Program " + pa.ProgramNumber + " is on PID " + pa.PacketIdentifier);
#endif
                var updatedPrograms = (from r in pat.Rows select r.ProgramNumber).ToArray();
                var existingPrograms = (from p in Programs select p.ProgramNumber).ToArray();

                var newProgramRows = from r in pat.Rows
                                     where updatedPrograms.Except(existingPrograms).Contains(r.ProgramNumber)
                                     select r;
                foreach (var r in newProgramRows)
                {
                    var newProgram = new TsProgram(r.ProgramNumber,
                                                   new TableStream() {PacketIdentifier = r.PacketIdentifier})
                                         {
                                             Status =
                                                 pat.IsCurrent ? TsProgramStatus.Current : TsProgramStatus.Next
                                         };
                    _streams.Add(r.PacketIdentifier, newProgram.ProgramMapStream);
                    Programs.Add(newProgram);
                    newProgram.StreamAdded += AddProgramStream;
                }

                if (pat.IsCurrent)
                {
                    var oldPrograms = from p in Programs
                                      where !updatedPrograms.Contains(p.ProgramNumber)
                                      select p;
                    foreach (var p in oldPrograms)
                    {
                        p.Status = TsProgramStatus.Dicontinued;
                    }
                }
            }
        }

        private void DescriptionTableReceived(object sender, TsStreamEventArgs eventArgs)
        {
            var e = eventArgs as TsStreamEventArgs<TsTable>;
            if (e == null) return;

            Debug.WriteLine("Received Description Table");
        }

        private void AddProgramStream(object sender, ProgramStreamEventArgs e)
        {
            _streams.Add(e.Stream.PacketIdentifier, e.Stream);
        }
    }
}
