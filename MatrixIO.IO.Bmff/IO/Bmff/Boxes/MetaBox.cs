using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Meta Box ("meta")
    /// </summary>
    [Box("meta", "Meta Box")]
    public sealed class MetaBox : FullBox, ISuperBox
    {
        public MetaBox() : base() { }
        public MetaBox(byte version, uint flags = 0) : base(version, flags) { }
        public MetaBox(Stream stream) : base(stream) { }

        public IList<Box> Children { get; } = new List<Box>();

        public IEnumerator<Box> GetEnumerator() => Children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Children.GetEnumerator();

        /* TODO: Support standard sub-boxes!
        public HandlerBox TheHandler
        {
            get => Children.OfType<HandlerBox>().FirstOrDefault();
        }

        public PrimaryItemBox PrimaryResource
        {
            get => Children.OfType<PrimaryItemBox>().FirstOrDefault();
        }
        */

        public DataInformationBox FileLocations
        {
            get => Children.OfType<DataInformationBox>().FirstOrDefault();
        }

        /*
        public ItemLocationBox ItemLocations
        {
            get => Children.OfType<ItemLocationBox>().FirstOrDefault();
        }
        */

        public ItemProtectionBox Protections
        {
            get => Children.OfType<ItemProtectionBox>().FirstOrDefault();
        }

        /*
        public ItemInfoBox ItemInfos
        {
            get => Children.OfType<ItemInfoBox>().FirstOrDefault();      
        }

        public IPMPControlBox IPMPControl
        {
            get => Children.OfType<IPMPControlBox>().FirstOrDefault();
        }
        */

        private Type[] StandardBoxes =
        {
            //typeof(HandlerBox),
            //typeof(PrimaryItemBox),
            typeof(DataInformationBox),
            //typeof(ItemLocationBox),
            typeof(ItemProtectionBox),
            //typeof(ItemInfoBox),
            //typeof(IPMPControlBox),
        };

        public IEnumerable<Box> OtherBoxes
        {
            get
            {
                return from c in Children
                       where !StandardBoxes.Contains(c.GetType())
                       select c;
            }
        }
    }
}