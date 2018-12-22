using System.IO;
using MatrixIO.IO.Numerics;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Track Clean Aperture Dimensions Atom ("clef")
    /// </summary>
    [Box("clef", "Track Clean Aperture Dimensions Atom")]
    public sealed class TrackCleanApertureDimensionsBox : FullBox
    {
        public TrackCleanApertureDimensionsBox() 
            : base() { }

        public TrackCleanApertureDimensionsBox(Stream stream) 
            : base(stream) { }

        /// <summary>
        /// 16.16 Fixed-Point Number
        /// </summary>
        public FixedPoint_16_16 Width { get; set; }

        /// <summary>
        /// 16.16 Fixed-Point Number
        /// </summary>
        public FixedPoint_16_16 Height { get; set; }

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + 4 + 4;
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            Width = stream.ReadBEUInt32();
            Height = stream.ReadBEUInt32();
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            stream.WriteBEUInt32(Width.Value);
            stream.WriteBEUInt32(Height.Value);
        }
    }
}