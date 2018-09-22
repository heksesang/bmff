﻿using System.Collections.Generic;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Bits Per Component Box ("bpcc")
    /// </summary>
    [Box("bpcc", "Bits Per Component Box")]
    public sealed class BitsPerComponentBox : Box, ITableBox<BitsPerComponentBox.ComponentBitsEntry>
    {
        public BitsPerComponentBox() : base() { }
        public BitsPerComponentBox(Stream stream) : base(stream) { }

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + (ulong)_Entries.Count;
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            // TODO: Read in as a byte array and convert to List<ComponentBitsEntry>
            while (stream.Position < (long)(Offset + EffectiveSize))
            {
                byte b = stream.ReadOneByte();
                _Entries.Add(new ComponentBitsEntry(b));
            }
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            // TODO: Convert to byte[] and write all at once.
            foreach (ComponentBitsEntry entry in _Entries)
            {
                stream.WriteOneByte(entry.ComponentBits);
            }
        }

        private IList<ComponentBitsEntry> _Entries = new List<ComponentBitsEntry>();

        public IList<ComponentBitsEntry> Entries => _Entries;

        public int EntryCount => _Entries.Count;

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