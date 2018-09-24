using System.Collections.Generic;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Chunk Offset Box ("stco")
    /// </summary>
    [Box("stco", "Chunk Offset Box")]
    public sealed class ChunkOffsetBox : FullBox, ITableBox<ChunkOffsetBox.ChunkOffsetEntry>
    {
        public ChunkOffsetBox() : base() { }
        public ChunkOffsetBox(Stream stream) : base(stream) { }

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + 4 + ((ulong)Entries.Count * 4);
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            uint entryCount = stream.ReadBEUInt32();

            for (uint i = 0; i < entryCount; i++)
            {
                Entries.Add(new ChunkOffsetEntry(stream.ReadBEUInt32()));
            }
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            stream.WriteBEUInt32((uint)Entries.Count);

            foreach (ChunkOffsetEntry chunkOffsetEntry in Entries)
            {
                stream.WriteBEUInt32(chunkOffsetEntry.ChunkOffset);
            }
        }

        public IList<ChunkOffsetEntry> Entries { get; } = new List<ChunkOffsetEntry>();

        public int EntryCount => Entries.Count;

        public readonly struct ChunkOffsetEntry
        {
            public ChunkOffsetEntry(uint chunkOffset)
            {
                ChunkOffset = chunkOffset;
            }

            public uint ChunkOffset { get; }

            public static implicit operator uint(ChunkOffsetEntry entry)
            {
                return entry.ChunkOffset;
            }

            public static implicit operator ChunkOffsetEntry(uint chunkOffset)
            {
                return new ChunkOffsetEntry(chunkOffset);
            }
        }
    }
}