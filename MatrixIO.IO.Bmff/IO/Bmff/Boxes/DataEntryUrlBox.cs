using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Data Entry Url Box ("url ")
    /// </summary>
    [Box("url ", "Data Entry Url Box")]
    public class DataEntryUrlBox : FullBox
    {
        public DataEntryUrlBox() : base() { }
        public DataEntryUrlBox(Stream stream) : base(stream) { }

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + (String.IsNullOrEmpty(Location) ? 0 : (ulong)Encoding.UTF8.GetByteCount(Location));
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            Location = stream.ReadNullTerminatedUTF8String();
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            if(!String.IsNullOrEmpty(Location))
                stream.WriteUTF8String(Location);
        }

        public string Location { get; set; }
    }
}
