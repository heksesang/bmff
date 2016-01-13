using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Compact Sample Size Box ("stz2")
    /// </summary>
    [Box("stz2", "Compact Sample Size Box")]
    public class CompactSampleSizeBox : FullBox, ITableBox<CompactSampleSizeBox.CompactSampleSizeEntry>
    {
        public CompactSampleSizeBox() : base() { }
        public CompactSampleSizeBox(Stream stream) : base(stream) { }

        internal override ulong CalculateSize()
        {
            ulong calculatedSize = base.CalculateSize() + 3 + 1;
            if (FieldSize == 4) calculatedSize += (ulong)Math.Ceiling((double)_Entries.Count / 2D);
            else if (FieldSize == 8) calculatedSize += (ulong)_Entries.Count;
            else if (FieldSize == 16) calculatedSize += (ulong)_Entries.Count * 2;
            return calculatedSize;
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            _Reserved = stream.ReadBEUInt24();
            _FieldSize = stream.ReadOneByte();
            uint entryCount = stream.ReadBEUInt32();

            for (uint i = 0; i < entryCount; i++)
            {
                if (FieldSize == 4)
                {
                    byte twoFieldSizes = stream.ReadOneByte();
                    _Entries.Add(new CompactSampleSizeEntry((ushort)(twoFieldSizes & 0xFF00 >> 4)));
                    if(i < entryCount)
                        _Entries.Add(new CompactSampleSizeEntry((ushort)(twoFieldSizes & 0x00FF)));
                }
                else if (FieldSize == 8)
                    _Entries.Add(new CompactSampleSizeEntry(stream.ReadOneByte()));
                else if (FieldSize == 16)
                    _Entries.Add(new CompactSampleSizeEntry(stream.ReadBEUInt16()));
            }
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            stream.WriteBEUInt24(_Reserved);
            stream.WriteByte(FieldSize);
            stream.WriteBEUInt32((uint)_Entries.Count);

            if (FieldSize == 4)
            {
                for (int i = 0; i < _Entries.Count; i += 2)
                {
                    byte twoFieldSizes = (byte)((_Entries[i].EntrySize & 0x00FF) << 4);
                    if (i + 1 < _Entries.Count)
                        twoFieldSizes |= (byte)(_Entries[i].EntrySize & 0x00FF);
                    stream.WriteByte(twoFieldSizes);
                }
            }
            else
            {
                foreach (CompactSampleSizeEntry compactSampleSizeEntry in _Entries)
                {
                    if (FieldSize == 8)
                        stream.WriteByte((byte)compactSampleSizeEntry.EntrySize);
                    else if (FieldSize == 16) 
                        stream.WriteBEUInt16(compactSampleSizeEntry.EntrySize);
                }
            }
        }

        private IList<CompactSampleSizeEntry> _Entries = Portability.CreateList<CompactSampleSizeEntry>();
        public IList<CompactSampleSizeEntry> Entries
        {
            get
            {
                return _Entries;
            }
        }

        private uint _Reserved;

        private byte _FieldSize;
        public byte FieldSize 
        {
            get
            {
                return _FieldSize;
            }
            set
            {
                if (_FieldSize != 4 && _FieldSize != 8 && _FieldSize != 16) throw new ArgumentOutOfRangeException("FieldSize must be 4, 8 or 16.");
                _FieldSize = value;
            }
        }

        public int EntryCount { get { return _Entries.Count; } }

        public class CompactSampleSizeEntry
        {
            public ushort EntrySize { get; set; }

            public CompactSampleSizeEntry() { }
            public CompactSampleSizeEntry(ushort entrySize)
            {
                EntrySize = entrySize;
            }

            public static implicit operator uint(CompactSampleSizeEntry entry)
            {
                return entry.EntrySize;
            }

            public static implicit operator CompactSampleSizeEntry(ushort entrySize)
            {
                return new CompactSampleSizeEntry(entrySize);
            }
        }
    }
}
