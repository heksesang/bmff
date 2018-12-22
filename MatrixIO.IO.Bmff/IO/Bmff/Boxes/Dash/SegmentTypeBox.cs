using System.Collections.Generic;
using System.IO;

namespace MatrixIO.IO.Bmff.IO.Bmff.Boxes.Dash
{
    /// <summary>
    /// Segment Type Box ("styp")
    /// </summary>
    [Box("styp", "Segment Type Box")]
    public class SegmentTypeBox : Box
    {
        public SegmentTypeBox()
            : base() { }

        public SegmentTypeBox(Stream stream)
            : base(stream) { }

        public SegmentTypeBox(FourCC majorBrand, uint minorVersion, FourCC[] compatibleBrands)
            : this()
        {
            MajorBrand = majorBrand;
            MinorVersion = minorVersion;
            foreach (FourCC fourCC in compatibleBrands)
            {
                CompatibleBrands.Add(fourCC);
            }
        }

        public FourCC MajorBrand { get; set; }

        public uint MinorVersion { get; set; }

        public List<FourCC> CompatibleBrands { get; } = new List<FourCC>();

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + 4 + 4 + ((ulong)CompatibleBrands.Count * 4);
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            MajorBrand = new FourCC(stream.ReadBEUInt32());
            MinorVersion = stream.ReadBEUInt32();

            ulong remainingBytes = EffectiveSize - CalculateSize();
            ulong numBrands = remainingBytes / 4;

            for (ulong i = 0; i < numBrands; i++)
            {
                CompatibleBrands.Add(new FourCC(stream.ReadBEUInt32()));
            }
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);
            stream.WriteBytes(MajorBrand.GetBytes());
            stream.WriteBEUInt32(MinorVersion);

            foreach (FourCC compatibleBrand in CompatibleBrands)
            {
                stream.WriteBytes(compatibleBrand.GetBytes());
            }
        }
    }
}
