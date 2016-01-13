using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Item Protection Box ("ipro")
    /// </summary>
    [Box("ipro", "Item Protection Box")]
    public class ItemProtectionBox : FullBox, ISuperBox
    {
        public ItemProtectionBox() : base() { }
        public ItemProtectionBox(Stream stream) : base(stream) { }

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + 2;
        }

        protected override void LoadFromStream(System.IO.Stream stream)
        {
            base.LoadFromStream(stream);

            ProtectionCount = stream.ReadBEUInt16();
        }

        protected override void SaveToStream(System.IO.Stream stream)
        {
            base.SaveToStream(stream);

            stream.WriteBEUInt16((ushort)Children.Count);
        }

        private IList<Box> _Children = Portability.CreateList<Box>();
        public IList<Box> Children
        {
            get { return _Children; }
        }

        public IEnumerator<Box> GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        private ushort ProtectionCount { get; set; }

        /*
        public IEnumerable<ProtectionSchemeInfoBox> ProtectionInformation
        {
            get
            {
                return from c in Children
                       where c is ProtectionSchemeInfoBox
                       select (ProtectionSchemeInfoBox)c;
            }
        }
        */
    }
}
