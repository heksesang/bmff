using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Time To Sample Box ("stts")
    /// </summary>
    [Box("stts", "Time To Sample Box")]
    public sealed class TimeToSampleBox : FullBox, ITableBox<TimeToSampleBox.TimeToSampleEntry>
    {
        public TimeToSampleBox()
            : base() { }

        public TimeToSampleBox(Stream stream)
            : base(stream) { }

        public TimeToSampleEntry[] Entries { get; set; }

        public int EntryCount => Entries.Length;

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + 4 + ((ulong)Entries.Length * (4 + 4));
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            uint entryCount = stream.ReadBEUInt32();

            this.Entries = new TimeToSampleEntry[entryCount];

            for (uint i = 0; i < entryCount; i++)
            {
                Entries[i] = new TimeToSampleEntry(
                    sampleCount: stream.ReadBEUInt32(),
                    sampleDelta: stream.ReadBEUInt32()
                );
            }
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            stream.WriteBEUInt32((uint)Entries.Length);

            for (var i = 0; i < Entries.Length; i++)
            {
                ref TimeToSampleEntry entry = ref Entries[i];

                stream.WriteBEUInt32(entry.SampleCount);
                stream.WriteBEUInt32(entry.SampleDelta);
            }
        }

        public readonly struct TimeToSampleEntry
        {
            public TimeToSampleEntry(uint sampleCount, uint sampleDelta)
            {
                SampleCount = sampleCount;
                SampleDelta = sampleDelta;
            }

            public uint SampleCount { get; }

            public uint SampleDelta { get; }
        }
    }
}