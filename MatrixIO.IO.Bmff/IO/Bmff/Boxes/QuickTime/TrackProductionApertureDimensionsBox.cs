using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Track Production Aperture Dimensions Atom ("prof")
    /// </summary>
    [Box("prof", "Track Production Aperture Dimensions Atom")]
    public class TrackProductionApertureDimensionsBox : FullBox
    {
        public TrackProductionApertureDimensionsBox() : base() { }
        public TrackProductionApertureDimensionsBox(Stream stream) : base(stream) { }

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

            stream.WriteBEUInt32(Width);
            stream.WriteBEUInt32(Height);
        }

        /// <summary>
        /// 16.16 Fixed-Point Number
        /// </summary>
        public uint Width { get; set; }
        public double WidthAsDouble
        {
            get
            {
                double decimalWidth = (double)Width;
                return decimalWidth / 65536;
            }
            set
            {
                Width = (uint)Math.Round(value * 65536, 0);
            }
        }

        /// <summary>
        /// 16.16 Fixed-Point Number
        /// </summary>
        public uint Height { get; set; }
        public double HeightAsDouble
        {
            get
            {
                double decimalHeight = (double)Height;
                return decimalHeight / 65536;
            }
            set
            {
                Height = (uint)Math.Round(value * 65536, 0);
            }
        }
    }
}
