using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Movie Hint Info Atom ("hnti")
    /// </summary>
    [Box("hnti", "Movie Hint Info Atom")]
    public sealed class MovieHintInfoBox : Box, ISuperBox
    {
        public MovieHintInfoBox() : base() { }
        public MovieHintInfoBox(Stream stream) : base(stream) { }

        public IList<Box> Children { get; } = new List<Box>();

        internal override ulong CalculateSize()
        {
            return base.CalculateSize();
        }

        protected override void SaveChildrenToStream(Stream stream)
        {
            base.SaveChildrenToStream(stream);
        }

        public IEnumerator<Box> GetEnumerator() => Children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Children.GetEnumerator();
    }
}