using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Image Header Box ("ihdr")
    /// </summary>
    [Box("ihdr", "Image Header Box")]
    public class ImageHeaderBox : Box
    {
        public ImageHeaderBox() : base() { }
        public ImageHeaderBox(Stream stream) : base(stream) { }

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + 4 + 4 + 2 + 1 + 1 + 1 + 1;
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            _Height = stream.ReadBEUInt32();
            _Width = stream.ReadBEUInt32();
            _NumberOfComponents = stream.ReadBEUInt16();
            BitsPerComponent = stream.ReadOneByte();
            _CompressionType = stream.ReadOneByte();
            _IsUnknownColorspace = stream.ReadOneByte();
            _IsIntellectualProperty = stream.ReadOneByte();
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            stream.WriteBEUInt32(_Height);
            stream.WriteBEUInt32(_Width);
            stream.WriteBEUInt16(_NumberOfComponents);
            stream.WriteOneByte(BitsPerComponent);
            stream.WriteOneByte(_CompressionType);
            stream.WriteOneByte(_IsUnknownColorspace);
            stream.WriteOneByte(_IsIntellectualProperty);
        }

        private uint _Height;
        public long Height { get { return _Height; } set { _Height = checked((uint)value); } }

        private uint _Width;
        public long Width { get { return _Width; } set { _Width = checked((uint)value); } }

        private ushort _NumberOfComponents;
        public int NumberOfComponents { get { return _NumberOfComponents; } set { _NumberOfComponents = checked((ushort)value); } }

        public byte BitsPerComponent { get; set; }

        private byte _CompressionType = 7; // Default value defined in 15444-1 Annex I
        public byte CompressionType { get { return _CompressionType; } set { _CompressionType = value; } }

        private byte _IsUnknownColorspace;
        public bool IsUnknownColorspace { get { return _IsUnknownColorspace > 0; } set { _IsUnknownColorspace = value ? (byte)0x01 : (byte)0x00; } }

        private byte _IsIntellectualProperty;
        public bool IsIntellectualProperty { get { return _IsIntellectualProperty > 0; } set { _IsIntellectualProperty = value ? (byte)0x01 : (byte)0x00; } }
    }
}
