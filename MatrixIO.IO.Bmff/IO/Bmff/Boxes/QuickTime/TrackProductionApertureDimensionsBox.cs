using System.IO;
using MatrixIO.IO.Numerics;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Track Production Aperture Dimensions Atom ("prof")
    /// </summary>
    [Box("prof", "Track Production Aperture Dimensions Atom")]
    public sealed class TrackProductionApertureDimensionsBox : FullBox
    {
        public TrackProductionApertureDimensionsBox() 
            : base() { }

        public TrackProductionApertureDimensionsBox(Stream stream)
            : base(stream) { }

        /// <summary>
        /// 16.16 Fixed-Point Number
        /// </summary>
        public FixedPoint_16_16 Width { get; set; }

        /// <summary>
        /// 16.16 Fixed-Point Number
        /// </summary>
        public FixedPoint_16_16 Height { get; set; }

        internal override ulong CalculateSize() => base.CalculateSize() + 4 + 4;
        
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