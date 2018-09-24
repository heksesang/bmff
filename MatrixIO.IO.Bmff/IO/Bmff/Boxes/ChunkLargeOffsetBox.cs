using System.Collections.Generic;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Chunk Large Offset Box ("co64")
    /// </summary>
    [Box("co64", "Chunk Large Offset Box")]
    public sealed class ChunkLargeOffsetBox : FullBox, ITableBox<ChunkLargeOffsetBox.ChunkLargeOffsetEntry>
    {
        public ChunkLargeOffsetBox()
            : base() { }

        public ChunkLargeOffsetBox(Stream stream) 
            : base(stream) { }

        public ChunkLargeOffsetEntry[] Entries { get; set; }

        public int EntryCount => Entries.Length;

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + 4 + ((ulong)Entries.Length * 8);
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            uint entryCount = stream.ReadBEUInt32();

            Entries = new ChunkLargeOffsetEntry[entryCount];

            for (uint i = 0; i < entryCount; i++)
            {
                Entries[i] = new ChunkLargeOffsetEntry(stream.ReadBEUInt64());
            }
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            stream.WriteBEUInt32((uint)Entries.Length);

            for (int i = 0; i < Entries.Length; i++)
            {
                ref ChunkLargeOffsetEntry entry = ref Entries[i];

                stream.WriteBEUInt64(entry.ChunkOffset);
            }
        }

        public readonly struct ChunkLargeOffsetEntry
        {
            public ChunkLargeOffsetEntry(ulong chunkOffset)
            {
                ChunkOffset = chunkOffset;
            }

            public ulong ChunkOffset { get; }

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