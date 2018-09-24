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

        public ChunkOffsetEntry[] Entries { get; set; }

        public int EntryCount => Entries.Length;

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + 4 + ((ulong)Entries.Length * 4);
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            uint entryCount = stream.ReadBEUInt32();

            Entries = new ChunkOffsetEntry[entryCount];

            for (uint i = 0; i < entryCount; i++)
            {
                Entries[i] = new ChunkOffsetEntry(stream.ReadBEUInt32());
            }
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            stream.WriteBEUInt32((uint)Entries.Length);

            for (int i = 0; i < Entries.Length; i++)
            {
                ref ChunkOffsetEntry entry = ref Entries[i];

                stream.WriteBEUInt32(entry.ChunkOffset);
            }
        }

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