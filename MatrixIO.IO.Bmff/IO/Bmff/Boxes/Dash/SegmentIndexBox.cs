using System.IO;

namespace MatrixIO.IO.Bmff.IO.Bmff.Boxes.Dash
{
    /// <summary>
    /// Segment Index Box ("sidx")
    /// </summary>
    [Box("sidx", "Segment Index Box")]
    public class SegmentIndexBox : FullBox
    {
        public SegmentIndexBox()
            : base() { }

        public SegmentIndexBox(Stream stream)
            : base(stream) { }

        public uint ReferenceId { get; set; }

        public uint Timescale { get; set; }

        public ulong EarliestPresentationTime { get; set; }

        public ulong FirstOffset { get; set; }

        public ushort Reserved1 { get; set; }

        public ushort ReferenceCount { get; set; }

        public SegmentIndexEntry[] Entries { get; set; }

        internal override ulong CalculateSize()
        {
            ulong size = base.CalculateSize() + 20;

            if (Version == 1)
            {
                size += 8;
            }

            size += (ulong)(SegmentIndexEntry.Size * ReferenceCount);

            return size;
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            ReferenceId = stream.ReadBEUInt32();
            Timescale = stream.ReadBEUInt32();

            if (Version == 1)
            {
                EarliestPresentationTime = stream.ReadBEUInt64();
                FirstOffset = stream.ReadBEUInt64();
            }
            else // v0
            {
                EarliestPresentationTime = stream.ReadBEUInt32();
                FirstOffset = stream.ReadBEUInt32();
            }

            Reserved1 = stream.ReadBEUInt16();
            ReferenceCount = stream.ReadBEUInt16();

            Entries = new SegmentIndexEntry[ReferenceCount];

            for (int i = 0; i < ReferenceCount; i++)
            {
                uint referenceValue = stream.ReadBEUInt32();
                uint duration = stream.ReadBEUInt32();
                uint sapValue = stream.ReadBEUInt32();

                Entries[i] = new SegmentIndexEntry(referenceValue, duration, sapValue);
            }
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            stream.WriteBEUInt32(ReferenceId);
            stream.WriteBEUInt32(Timescale);

            if (Version == 1)
            {
                stream.WriteBEUInt64(EarliestPresentationTime);
                stream.WriteBEUInt64(FirstOffset);
            }
            else // v0
            {
                stream.WriteBEUInt32((uint)EarliestPresentationTime);
                stream.WriteBEUInt32((uint)FirstOffset);
            }

            stream.WriteBEUInt16(Reserved1);
            stream.WriteBEUInt16(ReferenceCount);

            for (int i = 0; i < Entries.Length; i++)
            {
                ref SegmentIndexEntry entry = ref Entries[i];

                entry.WriteTo(stream);
            }
        }
    }

    public readonly struct SegmentIndexEntry
    {
        public const int Size = 12;

        private readonly uint referenceValue;
        private readonly uint sapValue;

        public SegmentIndexEntry(uint referenceValue, uint duration, uint sapValue)
        {
            this.referenceValue = referenceValue;
            this.Duration = duration;
            this.sapValue = sapValue;
        }

        public bool ReferenceType => (referenceValue >> 31) > 0;

        public uint ReferenceSize => (referenceValue & 0x7FFF_FFFF);

        public uint Duration { get; }

        public bool StartWithSap => (sapValue >> 31) > 0;

        public uint SapType => ((sapValue >> 28) & 0x07);

        public uint SapDeltaTime => (sapValue & 0x0FFF_FFFF);

        internal void WriteTo(Stream stream)
        {
            stream.WriteBEUInt32(referenceValue);
            stream.WriteBEUInt32(Duration);
            stream.WriteBEUInt32(sapValue);
        }
    }
}
