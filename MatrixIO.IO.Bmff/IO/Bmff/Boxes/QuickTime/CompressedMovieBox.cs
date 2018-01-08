using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Compressed Movie Atom ("moov")
    /// </summary>
    [Box("cmov", "Compressed Movie Atom")]
    public class CompressedMovieBox : Box, ISuperBox
    {
        public CompressedMovieBox() : base() { }
        public CompressedMovieBox(Stream stream) : base(stream) { }

        public IList<Box> Children { get; } = new List<Box>();

        public IEnumerator<Box> GetEnumerator() => Children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Children.GetEnumerator();
    }
}