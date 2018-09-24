using System.Collections.Generic;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Sample Dependency Type Box ("sdtp")
    /// </summary>
    [Box("sdtp", "Simple Dependency Type Box")]
    public sealed class SampleDependencyTypeBox : FullBox, ITableBox<SampleDependencyTypeBox.SampleDependencyEntry>
    {
        public SampleDependencyTypeBox() 
            : base() { }

        public SampleDependencyTypeBox(Stream stream) 
            : base(stream) { }

        public SampleDependencyEntry[] Entries { get; set; }

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + (ulong)Entries.Length;
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            var entries = new List<SampleDependencyEntry>();

            while (stream.Position < (long)(Offset + EffectiveSize))
            {
                byte b = stream.ReadOneByte();
                entries.Add(new SampleDependencyEntry(b));
            }

            Entries = entries.ToArray();
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            foreach (SampleDependencyEntry entry in Entries)
            {
                stream.WriteByte(entry.SampleDependency);
            }
        }

        public readonly struct SampleDependencyEntry
        {
            public SampleDependencyEntry(byte sampleDependency)
            {
                SampleDependency = sampleDependency;
            }

            public byte SampleDependency { get; }

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