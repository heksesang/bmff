using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Movie Extends Header Box ("mehd")
    /// </summary>
    [Box("mehd", "Movie Extends Header Box")]
    public class MovieExtendsHeaderBox : FullBox
    {
        public MovieExtendsHeaderBox() : base() { }
        public MovieExtendsHeaderBox(Stream stream) : base(stream) { }

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + (Version == 1 ? (ulong)8 : 4);
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            if (Version == 1) FragmentDuration = stream.ReadBEUInt64();
            else FragmentDuration = (ulong)stream.ReadBEInt32();
        }

        protected override void SaveToStream(Stream stream)
        {
            if(FragmentDuration > UInt32.MaxValue) Version = 1;

            base.SaveToStream(stream);

            if (Version == 1) stream.WriteBEUInt64(FragmentDuration);
            else stream.WriteBEUInt32((uint)FragmentDuration);
        }

        public ulong FragmentDuration { get; set; }
    }
}
