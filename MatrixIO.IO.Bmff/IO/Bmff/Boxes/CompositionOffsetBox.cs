using System.Collections.Generic;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Composition Offset Box ("ctts")
    /// </summary>
    [Box("ctts", "Composition Offset Box")]
    public sealed class CompositionOffsetBox : FullBox, ITableBox<CompositionOffsetBox.CompositionOffsetEntry>
    {
        public CompositionOffsetBox()
            : base() { }

        public CompositionOffsetBox(Stream stream) 
            : base(stream) { }

        public IList<CompositionOffsetEntry> Entries { get; } = new List<CompositionOffsetEntry>();

        public int EntryCount => Entries.Count;

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
                Entries.Add(new CompositionOffsetEntry(
                    sampleCount: stream.ReadBEUInt32(),
                    sampleOffset: stream.ReadBEUInt32()
                ));
            }
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            stream.WriteBEUInt32((uint)Entries.Count);

            foreach (CompositionOffsetEntry CompositionOffsetEntry in Entries)
            {
                stream.WriteBEUInt32(CompositionOffsetEntry.SampleCount);
                stream.WriteBEUInt32(CompositionOffsetEntry.SampleOffset);
            }
        }

        public readonly struct CompositionOffsetEntry
        {
            public CompositionOffsetEntry(uint sampleCount, uint sampleOffset)
            {
                SampleCount = sampleCount;
                SampleOffset = sampleOffset;
            }

            public uint SampleCount { get; }

            public uint SampleOffset { get; }
        }
    }
}