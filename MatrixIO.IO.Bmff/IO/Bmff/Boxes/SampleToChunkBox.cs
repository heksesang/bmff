using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Sample To Chunk Box ("stsc")
    /// </summary>
    [Box("stsc", "Sample To Chunk Box")]
    public sealed class SampleToChunkBox : FullBox, ITableBox<SampleToChunkBox.SampleToChunkEntry>
    {
        public SampleToChunkBox() 
            : base() { }

        public SampleToChunkBox(Stream stream) 
            : base(stream) { }

        public SampleToChunkEntry[] Entries { get; set; }

        public int EntryCount => Entries.Length;

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + 4 + ((ulong)Entries.Length * (4 + 4 + 4));
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            uint entryCount = stream.ReadBEUInt32();

            Entries = new SampleToChunkEntry[entryCount];

            for (uint i = 0; i < entryCount; i++)
            {
                Entries[i] = new SampleToChunkEntry(
                    firstChunk: stream.ReadBEUInt32(),
                    samplesPerChunk: stream.ReadBEUInt32(),
                    sampleDescriptionIndex: stream.ReadBEUInt32()
                );
            }
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            stream.WriteBEUInt32((uint)Entries.Length);

            for (int i = 0; i < Entries.Length; i++)
            {
                ref SampleToChunkEntry entry = ref Entries[i];

                stream.WriteBEUInt32(entry.FirstChunk);
                stream.WriteBEUInt32(entry.SamplesPerChunk);
                stream.WriteBEUInt32(entry.SampleDescriptionIndex);
            }
        }

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