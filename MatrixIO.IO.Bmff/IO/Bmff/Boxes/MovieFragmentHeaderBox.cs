using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MatrixIO.IO.Bmff;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Movie Fragment Header Box ("mfhd")
    /// </summary>
    [Box("mfhd", "Movie Fragment Header Box")]
    public class MovieFragmentHeaderBox : FullBox
    {
        public MovieFragmentHeaderBox() : base() { }
        public MovieFragmentHeaderBox(Stream stream) : base(stream) { }

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + 4;
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            SequenceNumber = stream.ReadBEUInt32();
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            stream.WriteBEUInt32(SequenceNumber);
        }
        public uint SequenceNumber { get; set; }
    }
}
