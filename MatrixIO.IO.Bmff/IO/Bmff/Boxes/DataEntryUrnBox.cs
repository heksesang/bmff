using System.IO;
using System.Text;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Data Entry Urn Box ("urn ")
    /// </summary>
    [Box("urn ", "Data Entry Urn Box")]
    public sealed class DataEntryUrnBox : FullBox
    {
        public DataEntryUrnBox() : base() { }
        public DataEntryUrnBox(Stream stream) : base(stream) { }

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() +
                (string.IsNullOrEmpty(Name) ? 0 : (ulong)Encoding.UTF8.GetByteCount(Name)) + 1 +
                (string.IsNullOrEmpty(Location) ? 0 : (ulong)Encoding.UTF8.GetByteCount(Location));
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            Name = stream.ReadNullTerminatedUTF8String();
            Location = stream.ReadNullTerminatedUTF8String();
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            stream.WriteNullTerminatedUTF8String(Name);
            if (Location.Length > 0)
                stream.WriteUTF8String(Location);
        }

        public string Name { get; set; }

        public string Location { get; set; }
    }
}
