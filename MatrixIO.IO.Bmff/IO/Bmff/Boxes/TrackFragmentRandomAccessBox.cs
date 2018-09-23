using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Track Fragment Random Access Box ("tfra")
    /// </summary>
    [Box("tfra", "Track Fragment Random Access Box")]
    public sealed class TrackFragmentRandomAccessBox : FullBox, ITableBox<TrackFragmentRandomAccessBox.TrackFragmentEntry>
    {
        private byte[] _reserved = new byte[4];
        private byte _sizeOf;
        private readonly List<TrackFragmentEntry> _entries = new List<TrackFragmentEntry>();

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
            _reserved = stream.ReadBytes(3);
            _sizeOf = stream.ReadOneByte();

            uint entryCount = stream.ReadBEUInt32();

            for (uint i = 0; i < entryCount; i++)
            {
                var entry = new TrackFragmentEntry();

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

                switch (SizeOfTrafNumber)
                {
                    case 1: entry.TrafNumber = stream.ReadOneByte(); break;
                    case 2: entry.TrafNumber = stream.ReadBEUInt16(); break;
                    case 3: entry.TrafNumber = stream.ReadBEUInt24(); break;
                    default: entry.TrafNumber = stream.ReadBEUInt32(); break;
                }

                switch (SizeOfTrunNumber)
                {
                    case 1: entry.TrunNumber = stream.ReadOneByte(); break;
                    case 2: entry.TrunNumber = stream.ReadBEUInt16(); break;
                    case 3: entry.TrunNumber = stream.ReadBEUInt24(); break;
                    default: entry.TrunNumber = stream.ReadBEUInt32(); break;
                }

                switch (SizeOfSampleNumber)
                {
                    case 1: entry.SampleNumber = stream.ReadOneByte(); break;
                    case 2: entry.SampleNumber = stream.ReadBEUInt16(); break;
                    case 3: entry.SampleNumber = stream.ReadBEUInt24(); break;
                    default: entry.SampleNumber = stream.ReadBEUInt32(); break;
                }

                _entries.Add(entry);
            }
        }

        protected override void SaveToStream(Stream stream)
        {
            sbyte GetIntSize(uint value)
            {
                if (value > 16777215) return 4;
                else if (value > ushort.MaxValue) return 3;
                else if (value > byte.MaxValue) return 2;
                else return 1;
            }

            bool has64BitEntry = false;

            foreach (var entry in _entries)
            {
                if (entry.Time > uint.MaxValue || entry.MoofOffset > uint.MaxValue)
                {
                    has64BitEntry = true;

                    break;
                }
            }            

            SizeOfTrafNumber   = GetIntSize(_entries.Max(e => e.TrafNumber));
            SizeOfTrunNumber   = GetIntSize(_entries.Max(e => e.TrunNumber));
            SizeOfSampleNumber = GetIntSize(_entries.Max(e => e.SampleNumber));
            Version            = has64BitEntry ? (byte)1 : (byte)0;

            base.SaveToStream(stream);

            stream.WriteBEUInt32(TrackID);
            stream.Write(_reserved, 0, 3);
            stream.WriteOneByte(_sizeOf);

            stream.WriteBEUInt32((uint)_entries.Count);

            foreach (var entry in _entries)
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

                switch (SizeOfTrafNumber)
                {
                    case 1: stream.WriteOneByte((byte)entry.TrafNumber); break;
                    case 2: stream.WriteBEUInt16((ushort)entry.TrafNumber); break;
                    case 3: stream.WriteBEUInt24((uint)entry.TrafNumber); break;
                    default: stream.WriteBEUInt32((uint)entry.TrafNumber); break;
                }

                switch (SizeOfTrunNumber)
                {
                    case 1: stream.WriteOneByte((byte)entry.TrunNumber); break;
                    case 2: stream.WriteBEUInt16((ushort)entry.TrunNumber); break;
                    case 3: stream.WriteBEUInt24((uint)entry.TrunNumber); break;
                    default: stream.WriteBEUInt32((uint)entry.TrunNumber); break;
                }

                switch (SizeOfSampleNumber)
                {
                    case 1: stream.WriteOneByte((byte)entry.SampleNumber); break;
                    case 2: stream.WriteBEUInt16((ushort)entry.SampleNumber); break;
                    case 3: stream.WriteBEUInt24((uint)entry.SampleNumber); break;
                    default: stream.WriteBEUInt32((uint)entry.SampleNumber); break;
                }
            }
        }

        public uint TrackID { get; set; }
        
        public sbyte SizeOfTrafNumber
        {
            get => (sbyte)(((_sizeOf & 0x30) >> 4) + 1);
            private set
            {
                if (value < 1 || value > 4)
                {
                    throw new OverflowException("SizeOfTrafNumber must be a value between 1 and 4.");
                }

                _sizeOf = (byte)((_sizeOf & 0xCF) | (((SizeOfTrafNumber - 1) & 0x30) << 4));
            }
        }
        public sbyte SizeOfTrunNumber
        {
            get => (sbyte)(((_sizeOf & 0x30) >> 4) + 1);
            private set
            {
                if (value < 1 || value > 4)
                {
                    throw new OverflowException("SizeOfTrunNumber must be a value between 1 and 4.");
                }

                _sizeOf = (byte)((_sizeOf & 0xF3) | ((SizeOfSampleNumber - 1) & 0x03));
            }
        }
        public sbyte SizeOfSampleNumber
        {
            get => (sbyte)(((_sizeOf & 0x30) >> 4) + 1);
            private set
            {
                if (value < 1 || value > 4)
                {
                    throw new OverflowException("SizeOfSampleNumber must be a value between 1 and 4.");
                }

                _sizeOf = (byte)((_sizeOf & 0xFC) | ((SizeOfTrunNumber - 1) & 0x0C << 2));
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

        public IList<TrackFragmentEntry> Entries => _entries;
    }
}
