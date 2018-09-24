using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Item Protection Box ("ipro")
    /// </summary>
    [Box("ipro", "Item Protection Box")]
    public sealed class ItemProtectionBox : FullBox, ISuperBox
    {
        public ItemProtectionBox() : base() { }
        public ItemProtectionBox(Stream stream) : base(stream) { }

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + 2;
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            ProtectionCount = stream.ReadBEUInt16();
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            stream.WriteBEUInt16((ushort)Children.Count);
        }

        public IList<Box> Children { get; } = new List<Box>();

        public IEnumerator<Box> GetEnumerator() => Children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Children.GetEnumerator();

        private ushort ProtectionCount { get; set; }

        /*
        public IEnumerable<ProtectionSchemeInfoBox> ProtectionInformation
        {
            get => Children.OfType<ProtectionSchemeInfoBox>();
        }
        */
    }
}