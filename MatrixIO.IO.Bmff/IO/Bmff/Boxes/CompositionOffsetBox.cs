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

        public CompositionOffsetEntry[] Entries { get; set; }

        public int EntryCount => Entries.Length;

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + 4 + ((ulong)Entries.Length * (4 + 4));
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            uint entryCount = stream.ReadBEUInt32();

            Entries = new CompositionOffsetEntry[entryCount];

            for (uint i = 0; i < entryCount; i++)
            {
                Entries[i] = new CompositionOffsetEntry(
                    sampleCount: stream.ReadBEUInt32(),
                    sampleOffset: stream.ReadBEUInt32()
                );
            }
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            stream.WriteBEUInt32((uint)Entries.Length);

            for (int i = 0; i < Entries.Length; i++)
            {
                ref CompositionOffsetEntry entry = ref Entries[i];

                stream.WriteBEUInt32(entry.SampleCount);
                stream.WriteBEUInt32(entry.SampleOffset);
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