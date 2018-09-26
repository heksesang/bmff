using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Sync Sample Box ("stss")
    /// </summary>
    [Box("stss", "Sync Sample Box")]
    public sealed class SyncSampleBox : FullBox, ITableBox<SyncSampleBox.SyncSampleEntry>
    {
        public SyncSampleBox()
            : base() { }

        public SyncSampleBox(Stream stream)
            : base(stream) { }

        public SyncSampleEntry[] Entries { get; set; }

        public int EntryCount => Entries.Length;

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + 4 + ((ulong)Entries.Length * 4);
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            uint entryCount = stream.ReadBEUInt32();

            Entries = new SyncSampleEntry[entryCount];

            for (uint i = 0; i < entryCount; i++)
            {
                Entries[i] = new SyncSampleEntry(stream.ReadBEUInt32());
            }
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            stream.WriteBEUInt32((uint)Entries.Length);

            for (int i = 0; i < Entries.Length; i++)
            {
                ref SyncSampleEntry entry = ref Entries[i];

                stream.WriteBEUInt32(entry.SampleNumber);
            }
        }

        public readonly struct SyncSampleEntry
        {
            public SyncSampleEntry(uint sampleNumber)
            {
                SampleNumber = sampleNumber;
            }

            public uint SampleNumber { get; }

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