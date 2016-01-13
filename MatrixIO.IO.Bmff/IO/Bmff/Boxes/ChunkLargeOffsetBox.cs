using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            return base.CalculateSize() + 4 + ((ulong)_Entries.Count * 8);
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            uint entryCount = stream.ReadBEUInt32();

            for (uint i = 0; i < entryCount; i++)
            {
                _Entries.Add(new ChunkLargeOffsetEntry(stream.ReadBEUInt64()));
            }
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            stream.WriteBEUInt32((uint)_Entries.Count);

            foreach (ChunkLargeOffsetEntry chunkLargeOffsetEntry in _Entries)
            {
                stream.WriteBEUInt64(chunkLargeOffsetEntry.ChunkOffset);
            }
        }

        private IList<ChunkLargeOffsetEntry> _Entries = Portability.CreateList<ChunkLargeOffsetEntry>();
        public IList<ChunkLargeOffsetEntry> Entries
        {
            get
            {
                return _Entries;
            }
        }

        public int EntryCount { get { return _Entries.Count; } }

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
