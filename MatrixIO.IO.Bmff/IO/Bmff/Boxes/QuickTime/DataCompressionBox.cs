using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Data Compression Atom ("dcom")
    /// </summary>
    [Box("dcom", "Data Compression Atom")]
    public class DataCompressionBox: Box
    {
        public DataCompressionBox() : base() { }
        public DataCompressionBox(Stream stream) : base(stream) { }

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + (String.IsNullOrEmpty(Format) ? 0 : (ulong)Encoding.UTF8.GetByteCount(Format));
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

        public FourCC Format { get; set; }
    }
}
