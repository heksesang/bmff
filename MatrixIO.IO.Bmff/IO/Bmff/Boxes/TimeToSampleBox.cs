using System.Collections.Generic;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Time To Sample Box ("stts")
    /// </summary>
    [Box("stts", "Time To Sample Box")]
    public sealed class TimeToSampleBox : FullBox, ITableBox<TimeToSampleBox.TimeToSampleEntry>
    {
        public TimeToSampleBox() : base() { }
        public TimeToSampleBox(Stream stream) : base(stream) { }

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + 4 + ((ulong)Entries.Count * (4 + 4));
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            uint entryCount = stream.ReadBEUInt32();

            var entries = new TimeToSampleEntry[entryCount];

            for (uint i = 0; i < entryCount; i++)
            {
                entries[i] = new TimeToSampleEntry(
                    sampleCount: stream.ReadBEUInt32(),
                    sampleDelta: stream.ReadBEUInt32()
                );
            }

            Entries = entries;
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            stream.WriteBEUInt32((uint)Entries.Count);

            foreach (TimeToSampleEntry TimeToSampleEntry in Entries)
            {
                stream.WriteBEUInt32(TimeToSampleEntry.SampleCount);
                stream.WriteBEUInt32(TimeToSampleEntry.SampleDelta);
            }
        }

        public IList<TimeToSampleEntry> Entries { get; private set; }

        public int EntryCount => Entries.Count;

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
