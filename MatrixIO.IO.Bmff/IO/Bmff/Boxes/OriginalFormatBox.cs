using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Original Format Box ("frma")
    /// </summary>
    [Box("frma", "Original Format Box")]
    public class OriginalFormatBox : FullBox
    {
        public OriginalFormatBox() : base() { }
        public OriginalFormatBox(Stream stream) : base(stream) { }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            _DataFormat = new FourCC(stream.ReadBEUInt32());
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            stream.WriteBEUInt32(_DataFormat);
        }

        public FourCC _DataFormat { get; set; }
    }
}
