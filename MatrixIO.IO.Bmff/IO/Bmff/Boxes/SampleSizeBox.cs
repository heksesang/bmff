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
            _SampleCount = stream.ReadBEUInt32();

            if (SampleSize == 0)
            {
                for (uint i = 0; i < _SampleCount; i++)
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
        private uint _SampleCount;
        public uint SampleCount
        {
            get
            {
                if (SampleSize == 0) return (uint)Entries.Count;
                return _SampleCount;
            }
        }

        public uint EntryCount
        {
            get
            {
                return (uint)Entries.Count;
            }
        }

        public class SampleSizeEntry
        {
            public uint EntrySize { get; set; }

            public SampleSizeEntry() { }
            public SampleSizeEntry(uint entrySize)
            {
                EntrySize = entrySize;
            }

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