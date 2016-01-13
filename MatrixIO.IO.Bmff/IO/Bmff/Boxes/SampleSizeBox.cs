using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Sample Size Box ("stsz")
    /// </summary>
    [Box("stsz", "Sample Size Box")]
    public class SampleSizeBox : FullBox, ITableBox<SampleSizeBox.SampleSizeEntry>
    {
        public SampleSizeBox() : base() { }
        public SampleSizeBox(Stream stream) : base(stream) { }

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + 4 + 4 + ((ulong)_Entries.Count * 4);
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
                    _Entries.Add(new SampleSizeEntry(stream.ReadBEUInt32()));
                }
            }
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            stream.WriteBEUInt32(SampleSize);
            stream.WriteBEUInt32(SampleCount);

            foreach (SampleSizeEntry SampleSizeEntry in _Entries)
            {
                stream.WriteBEUInt32(SampleSizeEntry.EntrySize);
            }
        }

        private IList<SampleSizeEntry> _Entries = Portability.CreateList<SampleSizeEntry>();
        public IList<SampleSizeEntry> Entries
        {
            get
            {
                return _Entries;
            }
        }

        public uint SampleSize { get; set; }
        private uint _SampleCount;
        public uint SampleCount 
        { 
            get 
            { 
                if(SampleSize == 0) return (uint)_Entries.Count;
                return _SampleCount;
            } 
        }

        public uint EntryCount
        {
            get
            {
                return (uint)_Entries.Count;
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
