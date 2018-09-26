using System;
using System.IO;

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

        public TrackFragmentRandomAccessBox()
            : base() { }

        public TrackFragmentRandomAccessBox(Stream stream)
            : base(stream) { }

        public TrackFragmentEntry[] Entries { get; set; }

        internal override ulong CalculateSize()
        {
            ulong entries = (ulong)Entries.Length;

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

            Entries = new TrackFragmentEntry[entryCount];

            for (uint i = 0; i < entryCount; i++)
            {
                ulong time;
                ulong moofOffset;
                uint trafNumber;
                uint trunNumber;
                uint sampleNumber;

                if (Version == 0x01)
                {
                    time = stream.ReadBEUInt64();
                    moofOffset = stream.ReadBEUInt64();
                }
                else
                {
                    time = stream.ReadBEUInt32();
                    moofOffset = stream.ReadBEUInt32();
                }

                switch (SizeOfTrafNumber)
                {
                    case 1: trafNumber = stream.ReadOneByte(); break;
                    case 2: trafNumber = stream.ReadBEUInt16(); break;
                    case 3: trafNumber = stream.ReadBEUInt24(); break;
                    default: trafNumber = stream.ReadBEUInt32(); break;
                }

                switch (SizeOfTrunNumber)
                {
                    case 1: trunNumber = stream.ReadOneByte(); break;
                    case 2: trunNumber = stream.ReadBEUInt16(); break;
                    case 3: trunNumber = stream.ReadBEUInt24(); break;
                    default: trunNumber = stream.ReadBEUInt32(); break;
                }

                switch (SizeOfSampleNumber)
                {
                    case 1: sampleNumber = stream.ReadOneByte(); break;
                    case 2: sampleNumber = stream.ReadBEUInt16(); break;
                    case 3: sampleNumber = stream.ReadBEUInt24(); break;
                    default: sampleNumber = stream.ReadBEUInt32(); break;
                }

                Entries[i] = new TrackFragmentEntry(time, moofOffset, trafNumber, trunNumber, sampleNumber);
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

            uint maxFrafNumber = 0;
            uint maxTrunNumber = 0;
            uint maxSampleNumber = 0;

            for (int i = 0; i < Entries.Length; i++)
            {
                ref TrackFragmentEntry entry = ref Entries[i];

                if (entry.TrafNumber > maxFrafNumber)
                {
                    maxFrafNumber = entry.TrafNumber;
                }

                if (entry.TrunNumber > maxTrunNumber)
                {
                    maxTrunNumber = entry.TrunNumber;
                }

                if (entry.SampleNumber > maxSampleNumber)
                {
                    maxSampleNumber = entry.SampleNumber;
                }

                if (!has64BitEntry && (entry.Time > uint.MaxValue || entry.MoofOffset > uint.MaxValue))
                {
                    has64BitEntry = true;
                }
            }

            SizeOfTrafNumber = GetIntSize(maxFrafNumber);
            SizeOfTrunNumber = GetIntSize(maxTrunNumber);
            SizeOfSampleNumber = GetIntSize(maxSampleNumber);
            Version = has64BitEntry ? (byte)1 : (byte)0;

            base.SaveToStream(stream);

            stream.WriteBEUInt32(TrackID);
            stream.Write(_reserved, 0, 3);
            stream.WriteOneByte(_sizeOf);

            stream.WriteBEUInt32((uint)Entries.Length);

            for (int i = 0; i < Entries.Length; i++)
            {
                ref TrackFragmentEntry entry = ref Entries[i];

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

        public readonly struct TrackFragmentEntry
        {
            public TrackFragmentEntry(ulong time, ulong moofOffset, uint trafNumber, uint trunNumber, uint sampleNumber)
            {
                Time = time;
                MoofOffset = moofOffset;
                TrafNumber = trafNumber;
                TrunNumber = trunNumber;
                SampleNumber = sampleNumber;
            }

            public ulong Time { get; }

            public ulong MoofOffset { get; }

            public uint TrafNumber { get; }

            public uint TrunNumber { get; }

            public uint SampleNumber { get; }
        }
    }
}
