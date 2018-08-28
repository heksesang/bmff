using System.IO;
using System.Text;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Data Entry Alias Box ("alis")
    /// </summary>
    [Box("alis", "Data Entry Alias Atom")]
    public sealed class DataEntryAliasBox : FullBox
    {
        public DataEntryAliasBox() : base() { }
        public DataEntryAliasBox(Stream stream) : base(stream) { }

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + (string.IsNullOrEmpty(Alias) ? 0 : (ulong)Encoding.UTF8.GetByteCount(Alias));
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            Alias = stream.ReadNullTerminatedUTF8String();
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            stream.WriteUTF8String(Alias);
        }

        public string Alias { get; set; }
    }
}