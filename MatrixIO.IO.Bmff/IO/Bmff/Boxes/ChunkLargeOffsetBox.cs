using System.Collections.Generic;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Chunk Large Offset Box ("co64")
    /// </summary>
    [Box("co64", "Chunk Large Offset Box")]
    public class ChunkLargeOffsetBox : FullBox, ITableBox<ChunkLargeOffsetBox.ChunkLargeOffsetEntry>
    {
        public ChunkLargeOffsetBox() : base() { }
        public ChunkLargeOffsetBox(Stream stream) : base(stream) { }

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + 4 + ((ulong)Entries.Count * 8);
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            uint entryCount = stream.ReadBEUInt32();

            for (uint i = 0; i < entryCount; i++)
            {
                Entries.Add(new ChunkLargeOffsetEntry(stream.ReadBEUInt64()));
            }
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            stream.WriteBEUInt32((uint)Entries.Count);

            foreach (ChunkLargeOffsetEntry chunkLargeOffsetEntry in Entries)
            {
                stream.WriteBEUInt64(chunkLargeOffsetEntry.ChunkOffset);
            }
        }

        public IList<ChunkLargeOffsetEntry> Entries { get; } = new List<ChunkLargeOffsetEntry>();

        public int EntryCount => Entries.Count;

        public class ChunkLargeOffsetEntry
        {
            public ulong ChunkOffset { get; set; }

            public ChunkLargeOffsetEntry() { }
            public ChunkLargeOffsetEntry(ulong chunkOffset)
            {
                ChunkOffset = chunkOffset;
            }

            public static implicit operator ulong(ChunkLargeOffsetEntry entry)
            {
                return entry.ChunkOffset;
            }
            public static implicit operator ChunkLargeOffsetEntry(ulong sampleDependency)
            {
                return new ChunkLargeOffsetEntry(sampleDependency);
            }
        }
    }
}