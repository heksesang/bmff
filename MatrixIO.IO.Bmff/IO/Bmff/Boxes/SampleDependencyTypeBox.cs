using System.Collections.Generic;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Sample Dependency Type Box ("sdtp")
    /// </summary>
    [Box("sdtp", "Simple Dependency Type Box")]
    public class SampleDependencyTypeBox : FullBox, ITableBox<SampleDependencyTypeBox.SampleDependencyEntry>
    {
        public SampleDependencyTypeBox() : base() { }
        public SampleDependencyTypeBox(Stream stream) : base(stream) { }

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + (ulong)Entries.Count;
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            while (stream.Position < (long)(Offset + EffectiveSize))
            {
                byte b = stream.ReadOneByte();
                Entries.Add(new SampleDependencyEntry(b));
            }
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            foreach (SampleDependencyEntry entry in Entries)
            {
                stream.WriteByte(entry.SampleDependency);
            }
        }

        public IList<SampleDependencyEntry> Entries { get; } = new List<SampleDependencyEntry>();

        public class SampleDependencyEntry
        {
            public byte SampleDependency { get; set; }

            public SampleDependencyEntry() { }
            public SampleDependencyEntry(byte sampleDependency)
            {
                SampleDependency = sampleDependency;
            }

            public static implicit operator byte(SampleDependencyEntry entry)
            {
                return entry.SampleDependency;
            }
            public static implicit operator SampleDependencyEntry(byte sampleDependency)
            {
                return new SampleDependencyEntry(sampleDependency);
            }
        }
    }
}
