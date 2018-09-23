using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Video Media Header Box ("vmhd")
    /// </summary>
    [Box("vmhd", "Video Media Header Box")]
    public sealed class VideoMediaHeaderBox : FullBox
    {
        public VideoMediaHeaderBox() : base() { }
        public VideoMediaHeaderBox(Stream stream) : base(stream) { }

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + 2 + 6;
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            GraphicsMode = (CompositionMode)stream.ReadBEUInt16();
            OpColour = new Colour(stream.ReadBEUInt16(), stream.ReadBEUInt16(), stream.ReadBEUInt16());
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            stream.WriteBEUInt16((ushort)GraphicsMode);
            stream.WriteBEUInt16(OpColour.Red);
            stream.WriteBEUInt16(OpColour.Green);
            stream.WriteBEUInt16(OpColour.Blue);
        }

        public CompositionMode GraphicsMode { get; set; }

        public Colour OpColour { get; set; }

        public enum CompositionMode : ushort
        {
            Copy = 0,
        }

        public readonly struct Colour
        {
            public Colour(ushort red, ushort green, ushort blue)
            {
                Red = red;
                Green = green;
                Blue = blue;
            }

            public ushort Red { get; }

            public ushort Green { get; }

            public ushort Blue { get; }

            public override string ToString()
            {
                return $"[0x{Red:X4}, 0x{Green:X4}, 0x{Blue:X4}]";
            }
        }
    }
}