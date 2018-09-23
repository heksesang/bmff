using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Movie Extends Header Box ("mehd")
    /// </summary>
    [Box("mehd", "Movie Extends Header Box")]
    public sealed class MovieExtendsHeaderBox : FullBox
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

            FragmentDuration = (Version == 1) ? stream.ReadBEUInt64() : (ulong)stream.ReadBEInt32();
        }

        protected override void SaveToStream(Stream stream)
        {
            if (FragmentDuration > uint.MaxValue)
            {
                Version = 1;
            }

            base.SaveToStream(stream);

            if (Version == 1)
            {
                stream.WriteBEUInt64(FragmentDuration);
            }
            else
            {
                stream.WriteBEUInt32((uint)FragmentDuration);
            }
        }

        public ulong FragmentDuration { get; set; }
    }
}