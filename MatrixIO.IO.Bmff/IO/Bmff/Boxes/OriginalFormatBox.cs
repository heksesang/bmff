using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Original Format Box ("frma")
    /// </summary>
    [Box("frma", "Original Format Box")]
    public sealed class OriginalFormatBox : Box
    {
        public OriginalFormatBox() 
            : base() { }

        public OriginalFormatBox(Stream stream) 
            : base(stream) { }

        public FourCC DataFormat { get; set; }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            DataFormat = new FourCC(stream.ReadBEUInt32());
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            stream.WriteBEUInt32(DataFormat);
        }
    }
}