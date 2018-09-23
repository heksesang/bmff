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
    public sealed class FileDeliveryItemInformationBox : FullBox, ISuperBox
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

            EntryCount = (ushort)Children.OfType<PartitionEntryBox>().Count();

            stream.WriteBEUInt16(EntryCount);
        }

        public IList<Box> Children { get; } = new List<Box>();

        public IEnumerator<Box> GetEnumerator() => Children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Children.GetEnumerator();

        ushort EntryCount { get; set; }

        public IEnumerable<PartitionEntryBox> PartitionEntries
        {
            get => Children.OfType<PartitionEntryBox>();
        }

        /*
        public FileDeliveySessionGroupBox SessionInfo
        {
            get => Children.OfType<FDSessionGroupBox>().FirstOrDefault();
        }

        public GroupIdToNameBox GroupIdToName
        {
            get => Children.OfType<GroupIdToNameBox>().FirstOrDefault();
        }
        */
    }
}