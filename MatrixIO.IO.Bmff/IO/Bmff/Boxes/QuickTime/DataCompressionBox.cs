using System;
using System.IO;
using System.Text;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Data Compression Atom ("dcom")
    /// </summary>
    [Box("dcom", "Data Compression Atom")]
    public sealed class DataCompressionBox : Box
    {
        public DataCompressionBox()
            : base() { }

        public DataCompressionBox(Stream stream)
            : base(stream) { }

        public FourCC Format { get; set; }

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + (string.IsNullOrEmpty(Format) ? 0 : (ulong)Encoding.UTF8.GetByteCount(Format));
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            Format = new FourCC(stream.ReadBytes(4));
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            stream.WriteBytes(Format.GetBytes());
        }
    }
}
