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

        public SampleSizeBox() 
            : base() { }

        public SampleSizeBox(Stream stream) 
            : base(stream) { }

        public uint SampleSize { get; set; }

        public uint SampleCount => (SampleSize == 0) ? (uint)Entries.Length : _sampleCount;

        public SampleSizeEntry[] Entries { get; set; }

        public uint EntryCount => (uint)Entries.Length;

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + 4 + 4 + ((ulong)Entries.Length * 4);
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            SampleSize = stream.ReadBEUInt32();
            _sampleCount = stream.ReadBEUInt32();

            Entries = new SampleSizeEntry[_sampleCount];

            if (SampleSize == 0)
            {
                for (int i = 0; i < _sampleCount; i++)
                {
                    Entries[i] = new SampleSizeEntry(stream.ReadBEUInt32());
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