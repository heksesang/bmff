using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Chunk Offset Box ("stco")
    /// </summary>
    [Box("stco", "Chunk Offset Box")]
    public class ChunkOffsetBox : FullBox, ITableBox<ChunkOffsetBox.ChunkOffsetEntry>
    {
        public ChunkOffsetBox() : base() { }
        public ChunkOffsetBox(Stream stream) : base(stream) { }

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + 4 + ((ulong)_Entries.Count * 4);
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            uint entryCount = stream.ReadBEUInt32();

            for (uint i = 0; i < entryCount; i++)
            {
                _Entries.Add(new ChunkOffsetEntry(stream.ReadBEUInt32()));
            }
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            stream.WriteBEUInt32((uint)_Entries.Count);

            foreach (ChunkOffsetEntry chunkOffsetEntry in _Entries)
            {
                stream.WriteBEUInt32(chunkOffsetEntry.ChunkOffset);
            }
        }

        private IList<ChunkOffsetEntry> _Entries = Portability.CreateList<ChunkOffsetEntry>();
        public IList<ChunkOffsetEntry> Entries 
        {
            get
            {
                return _Entries;
            }
        }

        public int EntryCount { get { return _Entries.Count; } }

        public class ChunkOffsetEntry
        {
            public uint ChunkOffset { get; set; }

            public ChunkOffsetEntry() { }
            public ChunkOffsetEntry(uint chunkOffset)
            {
                ChunkOffset = chunkOffset;
            }

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
