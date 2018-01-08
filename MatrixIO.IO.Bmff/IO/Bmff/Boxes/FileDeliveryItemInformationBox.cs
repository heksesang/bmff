using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// File Delivery Item Information Box ("fiin")
    /// </summary>
    [Box("fiin", "File Delivery Item Information Box")]
    public class FileDeliveryItemInformationBox : FullBox, ISuperBox
    {
        public FileDeliveryItemInformationBox() : base() { }
        public FileDeliveryItemInformationBox(Stream stream) : base(stream) { }

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + 2;
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            EntryCount = stream.ReadBEUInt16();
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            EntryCount = (ushort)(from c in Children
                                  where c is PartitionEntryBox
                                  select c).Count();

            stream.WriteBEUInt16(EntryCount);
        }

        public IList<Box> Children { get; } = new List<Box>();

        public IEnumerator<Box> GetEnumerator() => Children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Children.GetEnumerator();

        ushort EntryCount { get; set; }
        
        public IEnumerable<PartitionEntryBox> PartitionEntries
        {
            get
            {
                return from c in Children
                       where c is PartitionEntryBox
                       select (PartitionEntryBox)c;
            }
        }

        /*
        public FileDeliveySessionGroupBox SessionInfo
        {
            get
            {
                return (from c in Children
                        where c is FDSessionGroupBox
                        select (FDSessionGroupBox)c).FirstOrDefault();
            }
        }

        public GroupIdToNameBox GroupIdToName
        {
            get
            {
                return (from c in Children
                        where c is GroupIdToNameBox
                        select (GroupIdToNameBox)c).FirstOrDefault();
            }
        }
        */
    }
}