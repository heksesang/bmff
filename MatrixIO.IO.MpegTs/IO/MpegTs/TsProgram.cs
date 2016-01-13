using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MatrixIO.IO.MpegTs.Streams;
using MatrixIO.IO.MpegTs.Tables;

namespace MatrixIO.IO.MpegTs
{
    public enum TsProgramStatus
    {
        Current,
        Dicontinued,
        Next,
    }
    public class TsProgram
    {
        private readonly object _syncObject = new object();

        public ushort ProgramNumber { get; private set; }
        public TsProgramStatus Status { get; internal set; }
        public TableStream ProgramMapStream { get; internal set; }
        public IList<TsStream> Streams { get; private set; }
        public IList<TsDescriptor> Info { get; private set; }

        public event EventHandler<ProgramStreamEventArgs> StreamAdded;

        public TsProgram(ushort programNumber, TableStream programMapStream)
        {
            ProgramNumber = programNumber;
            ProgramMapStream = programMapStream;
            ProgramMapStream.UnitReceived += ProgramMapTableReceived;
            Streams = Portability.CreateList<TsStream>();
        }

        void ProgramMapTableReceived(object sender, TsStreamEventArgs eventArgs)
        {
            var e = eventArgs as TsStreamEventArgs<TsTable>;
            if (e == null) return;

            lock (_syncObject)
            {
                var pmt = e.DecodedUnit.TableIdentifier == TableIdentifier.ProgramMap ? (ProgramMapTable) e.DecodedUnit : null;
                if (pmt == null) return;

                Debug.WriteLine("Program Map Received");
                Debug.WriteLine("Program Number: " + pmt.ProgramNumber + " Program Clock Reference PID: " +
                                pmt.ProgramClockReferencePid);

                Info = pmt.ProgramInfo;
                foreach (var pm in pmt.Rows)
                {
                    Debug.WriteLine("******** " + pm.StreamType + " on PID " + pm.PacketIdentifier);
#if DEBUG
                    foreach (var descriptor in pm.StreamInfo)
                        Debug.WriteLine("\tHas " + descriptor.Tag + " descriptor.");
#endif
                    var stream =
                        (from s in Streams where s.PacketIdentifier == pm.PacketIdentifier select s).FirstOrDefault();
                    if (stream == null)
                    {
                        try
                        {
                            stream = (TsStream) Activator.CreateInstance(TsStream.StreamTypes[pm.StreamType]);
                            stream.PacketIdentifier = pm.PacketIdentifier;
                        }
                        catch (Exception)
                        {
                            stream = new ByteStream() {PacketIdentifier = pm.PacketIdentifier};
                        }
                        Streams.Add(stream);
                        OnStreamAdded(stream);
                    }
                    stream.Type = pm.StreamType;
                    stream.Info = pm.StreamInfo;
                }
            }
        }

        private void OnStreamAdded(TsStream stream)
        {
            if (StreamAdded != null) StreamAdded(this, new ProgramStreamEventArgs() { Stream = stream });
        }

        public override string ToString()
        {
            return "Program " + ProgramNumber;
        }
    }

    public class ProgramStreamEventArgs : EventArgs
    {
        public TsStream Stream { get; set; }
    }
}
