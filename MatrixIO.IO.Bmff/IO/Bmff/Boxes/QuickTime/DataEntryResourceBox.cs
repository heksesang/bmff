using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Data Entry Resource Box ("rsrc")
    /// </summary>
    [Box("rsrc", "Data Entry Resource Atom")]
    public class DataEntryResourceBox : FullBox
    {
        public DataEntryResourceBox() : base() { }
        public DataEntryResourceBox(Stream stream) : base(stream) { }

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + (String.IsNullOrEmpty(Alias) ? 0 : (ulong)Encoding.UTF8.GetByteCount(Alias)) + 1 + 4 + 2;
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            Alias = stream.ReadNullTerminatedUTF8String();
            ResourceType = stream.ReadBEUInt32();
            Id = stream.ReadBEInt16();
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            stream.WriteNullTerminatedUTF8String(Alias);
            stream.WriteBEUInt32(ResourceType);
            stream.WriteBEInt16(Id);
        }

        public string Alias { get; set; }
        public uint ResourceType { get; set; }
        public short Id { get; set; }
    }
}
