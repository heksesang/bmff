using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Image Header Box ("ihdr")
    /// </summary>
    [Box("ihdr", "Image Header Box")]
    public sealed class ImageHeaderBox : Box
    {
        private uint _height;
        private uint _width;
        private ushort _numberOfComponents;
        private byte _isUnknownColorSpace;
        private byte _isIntellectualProperty;

        public ImageHeaderBox()
            : base() { }

        public ImageHeaderBox(Stream stream)
            : base(stream) { }

        public long Height
        {
            get => _height;
            set => _height = checked((uint)value);
        }

        public long Width
        {
            get => _width;
            set => _width = checked((uint)value);
        }

        public int NumberOfComponents
        {
            get => _numberOfComponents;
            set => _numberOfComponents = checked((ushort)value);
        }

        public byte BitsPerComponent { get; set; }

        // Note: default value defined in 15444-1 Annex I
        public byte CompressionType { get; set; } = 7;

        public bool IsUnknownColorspace
        {
            get => _isUnknownColorSpace > 0;
            set => _isUnknownColorSpace = value ? (byte)0x01 : (byte)0x00;
        }

        public bool IsIntellectualProperty
        {
            get => _isIntellectualProperty > 0;
            set => _isIntellectualProperty = value ? (byte)0x01 : (byte)0x00;
        }

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + 4 + 4 + 2 + 1 + 1 + 1 + 1;
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            _height = stream.ReadBEUInt32();
            _width = stream.ReadBEUInt32();
            _numberOfComponents = stream.ReadBEUInt16();
            BitsPerComponent = stream.ReadOneByte();
            CompressionType = stream.ReadOneByte();
            _isUnknownColorSpace = stream.ReadOneByte();
            _isIntellectualProperty = stream.ReadOneByte();
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            stream.WriteBEUInt32(_height);
            stream.WriteBEUInt32(_width);
            stream.WriteBEUInt16(_numberOfComponents);
            stream.WriteOneByte(BitsPerComponent);
            stream.WriteOneByte(CompressionType);
            stream.WriteOneByte(_isUnknownColorSpace);
            stream.WriteOneByte(_isIntellectualProperty);
        }
    }
}
