using System.Collections.Generic;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Time To Sample Box ("stts")
    /// </summary>
    [Box("stts", "Time To Sample Box")]
    public class TimeToSampleBox : FullBox, ITableBox<TimeToSampleBox.TimeToSampleEntry>
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

            for (uint i = 0; i < entryCount; i++)
            {
                Entries.Add(new TimeToSampleEntry() {
                    SampleCount = stream.ReadBEUInt32(), 
                    SampleDelta = stream.ReadBEUInt32(),
                });
            }
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

        public IList<TimeToSampleEntry> Entries { get; } = new List<TimeToSampleEntry>();

        public int EntryCount { get { return Entries.Count; } }

        public class TimeToSampleEntry
        {
            public uint SampleCount { get; set; }
            public uint SampleDelta { get; set; }

            public TimeToSampleEntry() { }
            public TimeToSampleEntry(uint sampleCount, uint sampleDelta)
            {
                SampleCount = sampleCount;
                SampleDelta = sampleDelta;
            }
        }
    }
}
