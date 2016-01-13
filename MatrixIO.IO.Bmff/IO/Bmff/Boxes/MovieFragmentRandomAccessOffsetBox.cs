using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Movie Fragment Random Access Offset Box ("mfro")
    /// </summary>
    [Box("mfro", "Movie Fragment Random Access Offset Box")]
    public class MovieFragmentRandomAccessOffsetBox : FullBox
    {
        public MovieFragmentRandomAccessOffsetBox() : base() { }
        public MovieFragmentRandomAccessOffsetBox(Stream stream) : base(stream) { }

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + 4;
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            MfraSize = stream.ReadBEUInt32();
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            stream.WriteBEUInt32(MfraSize);
        }

        public uint MfraSize { get; set; }
    }
}
