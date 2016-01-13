using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Color Specification Box ("colr")
    /// </summary>
    [Box("colr", "Color Specification Box")]
    public class ColourSpecificationBox : Box, IContentBox
    {
        public ColourSpecificationBox() : base() { }
        public ColourSpecificationBox(Stream stream) : base(stream) { }

        internal override ulong CalculateSize()
        {
            ulong size = base.CalculateSize() + 1 + 1 + 1;
            if (SpecificationMethod == SpecificationMethods.EnumeratedColorspace) size += 4;
            return size;
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            SpecificationMethod = (SpecificationMethods)stream.ReadOneByte();
            Precedence = stream.ReadOneByte();
            ColorspaceApproximation = stream.ReadOneByte();
            if(SpecificationMethod == SpecificationMethods.EnumeratedColorspace)
                EnumeratedColorspace = (EnumeratedColorspaces)stream.ReadBEUInt32();
            // TODO: Read Profile?
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            stream.WriteOneByte((byte)SpecificationMethod);
            stream.WriteOneByte(Precedence);
            stream.WriteOneByte(ColorspaceApproximation);
            if (SpecificationMethod == SpecificationMethods.EnumeratedColorspace)
                stream.WriteBEUInt32((uint)EnumeratedColorspace);
            // TODO: Write Profile
        }

        public SpecificationMethods SpecificationMethod { get; set; }

        public byte Precedence { get; set; }

        public byte ColorspaceApproximation { get; set; }

        public EnumeratedColorspaces EnumeratedColorspace { get; set; }

        public enum SpecificationMethods : byte
        {
            EnumeratedColorspace = 1,
            RestrictedICCProfile = 2,
        }

        public enum EnumeratedColorspaces : uint
        {
            sRGB = 16,
            Greyscale = 17,
            sYCC = 18,
        }
    }
}
