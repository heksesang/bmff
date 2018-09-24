using System.Collections.Generic;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Bits Per Component Box ("bpcc")
    /// </summary>
    [Box("bpcc", "Bits Per Component Box")]
    public sealed class BitsPerComponentBox : Box, ITableBox<BitsPerComponentBox.ComponentBitsEntry>
    {
        public BitsPerComponentBox() 
            : base() { }

        public BitsPerComponentBox(Stream stream) 
            : base(stream) { }

        public ComponentBitsEntry[] Entries { get; set; }

        public int EntryCount => Entries.Length;

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + (ulong)Entries.Length;
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            var entries = new List<ComponentBitsEntry>();

            // TODO: Read in as a byte array and convert to List<ComponentBitsEntry>
            while (stream.Position < (long)(Offset + EffectiveSize))
            {
                byte b = stream.ReadOneByte();

                entries.Add(new ComponentBitsEntry(b));
            }

            Entries = entries.ToArray();
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            var data = new byte[Entries.Length];

            for (var i = 0; i < Entries.Length; i++)
            {
                data[i] = Entries[i].ComponentBits;
            }

            stream.Write(data, 0, data.Length);
        }

        public readonly struct ComponentBitsEntry
        {
            public ComponentBitsEntry(byte componentBits)
            {
                ComponentBits = componentBits;
            }

            public byte ComponentBits { get; }

            public static implicit operator byte(ComponentBitsEntry entry)
            {
                return entry.ComponentBits;
            }

            public static implicit operator ComponentBitsEntry(byte componentBits)
            {
                return new ComponentBitsEntry(componentBits);
            }
        }
    }
}