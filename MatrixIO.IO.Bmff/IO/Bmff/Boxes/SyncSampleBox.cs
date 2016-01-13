using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Sync Sample Box ("stss")
    /// </summary>
    [Box("stss", "Sync Sample Box")]
    public class SyncSampleBox : FullBox, ITableBox<SyncSampleBox.SyncSampleEntry>
    {
        public SyncSampleBox() : base() { }
        public SyncSampleBox(Stream stream) : base(stream) { }

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
                _Entries.Add(new SyncSampleEntry(stream.ReadBEUInt32()));
            }
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            stream.WriteBEUInt32((uint)_Entries.Count);

            foreach (SyncSampleEntry SyncSampleEntry in _Entries)
            {
                stream.WriteBEUInt32(SyncSampleEntry.SampleNumber);
            }
        }

        private IList<SyncSampleEntry> _Entries = Portability.CreateList<SyncSampleEntry>();
        public IList<SyncSampleEntry> Entries
        {
            get
            {
                return _Entries;
            }
        }

        public int EntryCount { get { return _Entries.Count; } }

        public class SyncSampleEntry
        {
            public uint SampleNumber { get; set; }

            public SyncSampleEntry() { }
            public SyncSampleEntry(uint sampleNumber)
            {
                SampleNumber = sampleNumber;
            }

            public static implicit operator uint(SyncSampleEntry entry)
            {
                return entry.SampleNumber;
            }
            public static implicit operator SyncSampleEntry(uint sampleDependency)
            {
                return new SyncSampleEntry(sampleDependency);
            }
        }
    }
}
