using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Track Fragment Random Access Box ("tfra")
    /// </summary>
    [Box("tfra", "Track Fragment Random Access Box")]
    public class TrackFragmentRandomAccessBox : FullBox, ITableBox<TrackFragmentRandomAccessBox.TrackFragmentEntry>
    {
        public TrackFragmentRandomAccessBox() : base() { }
        public TrackFragmentRandomAccessBox(Stream stream) : base(stream) { }

        internal override ulong CalculateSize()
        {
            ulong entries = (ulong)Entries.Count;
            return base.CalculateSize()
                + 4 + 4 + 4
                + (entries * (ulong)(Version == 0x01 ? 16 : 8))
                + (entries * (ulong)SizeOfTrafNumber)
                + (entries * (ulong)SizeOfTrunNumber)
                + (entries * (ulong)SizeOfSampleNumber);
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            TrackID = stream.ReadBEUInt32();
            Reserved = stream.ReadBytes(3);
            _SizeOf = stream.ReadOneByte();

            uint NumberOfEntries = stream.ReadBEUInt32();
            for (uint i = 0; i < NumberOfEntries; i++)
            {
                TrackFragmentEntry entry = new TrackFragmentEntry();
                if (Version == 0x01)
                {
                    entry.Time = stream.ReadBEUInt64();
                    entry.MoofOffset = stream.ReadBEUInt64();
                }
                else
                {
                    entry.Time = stream.ReadBEUInt32();
                    entry.MoofOffset = stream.ReadBEUInt32();
                }

                if (SizeOfTrafNumber == 1) entry.TrafNumber = stream.ReadOneByte();
                else if (SizeOfTrafNumber == 2) entry.TrafNumber = stream.ReadBEUInt16();
                else if (SizeOfTrafNumber == 3) entry.TrafNumber = stream.ReadBEUInt24();
                else entry.TrafNumber = stream.ReadBEUInt32();
              
                if (SizeOfTrunNumber == 1) entry.TrunNumber = stream.ReadOneByte();
                else if (SizeOfTrunNumber == 2) entry.TrunNumber = stream.ReadBEUInt16();
                else if (SizeOfTrunNumber == 3) entry.TrunNumber = stream.ReadBEUInt24();
                else entry.TrunNumber = stream.ReadBEUInt32();

                if (SizeOfSampleNumber == 1) entry.SampleNumber = stream.ReadOneByte();
                else if (SizeOfSampleNumber == 2) entry.SampleNumber = stream.ReadBEUInt16();
                else if (SizeOfSampleNumber == 3) entry.SampleNumber = stream.ReadBEUInt24();
                else entry.SampleNumber = stream.ReadBEUInt32();

                _Entries.Add(entry);
            }
        }

        protected override void SaveToStream(Stream stream)
        {
            if ((from entry in _Entries select Math.Max(entry.Time, entry.MoofOffset)).Max() > UInt32.MaxValue) Version = 1;
            else Version = 0;

            uint MaxTrafNumber = (from entry in _Entries select entry.TrafNumber).Max();
            if (MaxTrafNumber > 16777215) SizeOfTrafNumber = 4;
            else if (MaxTrafNumber > UInt16.MaxValue) SizeOfTrafNumber = 3;
            else if (MaxTrafNumber > Byte.MaxValue) SizeOfTrafNumber = 2;
            else SizeOfTrafNumber = 1;

            uint MaxTrunNumber = (from entry in _Entries select entry.TrunNumber).Max();
            if (MaxTrunNumber > 16777215) SizeOfTrunNumber = 4;
            else if (MaxTrunNumber > UInt16.MaxValue) SizeOfTrunNumber = 3;
            else if (MaxTrunNumber > Byte.MaxValue) SizeOfTrunNumber = 2;
            else SizeOfTrunNumber = 1;

            uint MaxSampleNumber = (from entry in _Entries select entry.SampleNumber).Max();
            if (MaxSampleNumber > 16777215) SizeOfSampleNumber = 4;
            else if (MaxSampleNumber > UInt16.MaxValue) SizeOfSampleNumber = 3;
            else if (MaxSampleNumber > Byte.MaxValue) SizeOfSampleNumber = 2;
            else SizeOfSampleNumber = 1;

            base.SaveToStream(stream);

            stream.WriteBEUInt32(TrackID);
            stream.Write(Reserved, 0, 3);
            stream.WriteOneByte(_SizeOf);

            stream.WriteBEUInt32((uint)_Entries.Count);
            foreach(var entry in _Entries)
            {
                if (Version == 0x01)
                {
                    stream.WriteBEUInt64(entry.Time);
                    stream.WriteBEUInt64(entry.MoofOffset);
                }
                else
                {
                    stream.WriteBEUInt32((uint)entry.Time);
                    stream.WriteBEUInt32((uint)entry.MoofOffset);
                }

                if (SizeOfTrafNumber == 1) stream.WriteOneByte((byte)entry.TrafNumber);
                else if (SizeOfTrafNumber == 2) stream.WriteBEUInt16((ushort)entry.TrafNumber);
                else if (SizeOfTrafNumber == 3) stream.WriteBEUInt24((uint)entry.TrafNumber);
                else stream.WriteBEUInt32((uint)entry.TrafNumber);

                if (SizeOfTrunNumber == 1) stream.WriteOneByte((byte)entry.TrunNumber);
                else if (SizeOfTrunNumber == 2) stream.WriteBEUInt16((ushort)entry.TrunNumber);
                else if (SizeOfTrunNumber == 3) stream.WriteBEUInt24((uint)entry.TrunNumber);
                else stream.WriteBEUInt32((uint)entry.TrunNumber);

                if (SizeOfSampleNumber == 1) stream.WriteOneByte((byte)entry.SampleNumber);
                else if (SizeOfSampleNumber == 2) stream.WriteBEUInt16((ushort)entry.SampleNumber);
                else if (SizeOfSampleNumber == 3) stream.WriteBEUInt24((uint)entry.SampleNumber);
                else stream.WriteBEUInt32((uint)entry.SampleNumber);
            }
        }

        public uint TrackID { get; set; }
        private byte[] Reserved = new byte[4];

        private byte _SizeOf;
        public sbyte SizeOfTrafNumber 
        {
            get
            {
                return (sbyte)(((_SizeOf & 0x30) >> 4) + 1);
            }
            private set
            {
                if (value < 1 || value > 4) throw new OverflowException("SizeOfTrafNumber must be a value between 1 and 4.");
                _SizeOf = (byte)((_SizeOf & 0xCF) | (((SizeOfTrafNumber - 1) & 0x30) << 4));
            }
        }
        public sbyte SizeOfTrunNumber
        {
            get
            {
                return (sbyte)(((_SizeOf & 0x30) >> 4) + 1);
            }
            private set
            {
                if (value < 1 || value > 4) throw new OverflowException("SizeOfTrunNumber must be a value between 1 and 4.");
                _SizeOf = (byte)((_SizeOf & 0xF3) | ((SizeOfSampleNumber - 1) & 0x03));
            }
        }
        public sbyte SizeOfSampleNumber
        {
            get
            {
                return (sbyte)(((_SizeOf & 0x30) >> 4) + 1);
            }
            private set
            {
                if (value < 1 || value > 4) throw new OverflowException("SizeOfSampleNumber must be a value between 1 and 4.");
                _SizeOf = (byte)((_SizeOf & 0xFC) | ((SizeOfTrunNumber - 1) & 0x0C << 2));
            }
        }

        public class TrackFragmentEntry
        {
            public TrackFragmentEntry() { }

            public ulong Time { get; set; }
            public ulong MoofOffset { get; set; }
            public uint TrafNumber { get; set; }
            public uint TrunNumber { get; set; }
            public uint SampleNumber { get; set; }
        }

        private IList<TrackFragmentEntry> _Entries = Portability.CreateList<TrackFragmentEntry>();
        public IList<TrackFragmentEntry> Entries
        {
            get
            {
                return _Entries;
            }
        }
    }
}
