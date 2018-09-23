using System.Collections.Generic;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Sample To Chunk Box ("stsc")
    /// </summary>
    [Box("stsc", "Sample To Chunk Box")]
    public sealed class SampleToChunkBox : FullBox, ITableBox<SampleToChunkBox.SampleToChunkEntry>
    {
        public SampleToChunkBox() : base() { }
        public SampleToChunkBox(Stream stream) : base(stream) { }

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + 4 + ((ulong)Entries.Count * (4 + 4 + 4));
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            uint entryCount = stream.ReadBEUInt32();

            for (uint i = 0; i < entryCount; i++)
            {
                Entries.Add(new SampleToChunkEntry(
                    firstChunk: stream.ReadBEUInt32(),
                    samplesPerChunk: stream.ReadBEUInt32(),
                    sampleDescriptionIndex: stream.ReadBEUInt32()
                ));
            }
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            stream.WriteBEUInt32((uint)Entries.Count);

            foreach (SampleToChunkEntry SampleToChunkEntry in Entries)
            {
                stream.WriteBEUInt32(SampleToChunkEntry.FirstChunk);
                stream.WriteBEUInt32(SampleToChunkEntry.SamplesPerChunk);
                stream.WriteBEUInt32(SampleToChunkEntry.SampleDescriptionIndex);
            }
        }

        public IList<SampleToChunkEntry> Entries { get; } = new List<SampleToChunkEntry>();

        public int EntryCount => Entries.Count;

        public readonly struct SampleToChunkEntry
        {
            public SampleToChunkEntry(uint firstChunk, uint samplesPerChunk, uint sampleDescriptionIndex)
            {
                FirstChunk = firstChunk;
                SamplesPerChunk = samplesPerChunk;
                SampleDescriptionIndex = sampleDescriptionIndex;
            }

            public uint FirstChunk { get; }

            public uint SamplesPerChunk { get; }

            public uint SampleDescriptionIndex { get; }
        }
    }
}
