using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Data Entry Urn Box ("urn ")
    /// </summary>
    [Box("urn ", "Data Entry Urn Box")]
    public class DataEntryUrnBox : FullBox
    {
        public DataEntryUrnBox() : base() { }
        public DataEntryUrnBox(Stream stream) : base(stream) { }

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + 
                (String.IsNullOrEmpty(Name) ? 0 : (ulong)Encoding.UTF8.GetByteCount(Name)) + 1 + 
                (String.IsNullOrEmpty(Location) ? 0 : (ulong)Encoding.UTF8.GetByteCount(Location));
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
            if(Location.Length > 0)
                stream.WriteUTF8String(Location);
        }

        public string Name { get; set; }
        public string Location { get; set; }
    }
}
