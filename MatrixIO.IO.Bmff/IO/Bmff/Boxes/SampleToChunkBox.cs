using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Sample To Chunk Box ("stsc")
    /// </summary>
    [Box("stsc", "Sample To Chunk Box")]
    public class SampleToChunkBox : FullBox, ITableBox<SampleToChunkBox.SampleToChunkEntry>
    {
        public SampleToChunkBox() : base() { }
        public SampleToChunkBox(Stream stream) : base(stream) { }

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + 4 + ((ulong)_Entries.Count * (4 + 4 + 4));
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            uint entryCount = stream.ReadBEUInt32();

            for (uint i = 0; i < entryCount; i++)
            {
                _Entries.Add(new SampleToChunkEntry()
                {
                    FirstChunk = stream.ReadBEUInt32(),
                    SamplesPerChunk = stream.ReadBEUInt32(),
                    SampleDescriptionIndex = stream.ReadBEUInt32(),
                });
            }
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            stream.WriteBEUInt32((uint)_Entries.Count);

            foreach (SampleToChunkEntry SampleToChunkEntry in _Entries)
            {
                stream.WriteBEUInt32(SampleToChunkEntry.FirstChunk);
                stream.WriteBEUInt32(SampleToChunkEntry.SamplesPerChunk);
                stream.WriteBEUInt32(SampleToChunkEntry.SampleDescriptionIndex);
            }
        }

        private IList<SampleToChunkEntry> _Entries = Portability.CreateList<SampleToChunkEntry>();
        public IList<SampleToChunkEntry> Entries
        {
            get
            {
                return _Entries;
            }
        }

        public int EntryCount { get { return _Entries.Count; } }

        public class SampleToChunkEntry
        {
            public uint FirstChunk { get; set; }
            public uint SamplesPerChunk { get; set; }
            public uint SampleDescriptionIndex { get; set; }

            public SampleToChunkEntry() { }
            public SampleToChunkEntry(uint firstChunk, uint samplesPerChunk, uint sampleDescriptionIndex)
            {
                FirstChunk = firstChunk;
                SamplesPerChunk = samplesPerChunk;
                SampleDescriptionIndex = sampleDescriptionIndex;
            }
        }
    }
}
