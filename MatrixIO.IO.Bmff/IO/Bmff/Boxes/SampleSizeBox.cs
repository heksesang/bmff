using System.Collections.Generic;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Sample Size Box ("stsz")
    /// </summary>
    [Box("stsz", "Sample Size Box")]
    public sealed class SampleSizeBox : FullBox, ITableBox<SampleSizeBox.SampleSizeEntry>
    {
        private uint _sampleCount;

        public SampleSizeBox() : base() { }
        public SampleSizeBox(Stream stream) : base(stream) { }

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + 4 + 4 + ((ulong)Entries.Count * 4);
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            SampleSize = stream.ReadBEUInt32();
            _sampleCount = stream.ReadBEUInt32();

            if (SampleSize == 0)
            {
                for (uint i = 0; i < _sampleCount; i++)
                {
                    Entries.Add(new SampleSizeEntry(stream.ReadBEUInt32()));
                }
            }
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            stream.WriteBEUInt32(SampleSize);
            stream.WriteBEUInt32(SampleCount);

            foreach (SampleSizeEntry SampleSizeEntry in Entries)
            {
                stream.WriteBEUInt32(SampleSizeEntry.EntrySize);
            }
        }

        public IList<SampleSizeEntry> Entries { get; } = new List<SampleSizeEntry>();

        public uint SampleSize { get; set; }

        public uint SampleCount => (SampleSize == 0) ? (uint)Entries.Count : _sampleCount;

        public uint EntryCount => (uint)Entries.Count;

        public readonly struct SampleSizeEntry
        {
            public SampleSizeEntry(uint entrySize)
            {
                EntrySize = entrySize;
            }

            public uint EntrySize { get; }

            public static implicit operator uint(SampleSizeEntry entry)
            {
                return entry.EntrySize;
            }

            public static implicit operator SampleSizeEntry(uint entrySize)
            {
                return new SampleSizeEntry(entrySize);
            }
        }
    }
}