using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Movie Box ("moov")
    /// </summary>
    [Box("moov", "Movie Box")]
    public sealed class MovieBox : Box, ISuperBox
    {
        public MovieBox() : base() { }
        public MovieBox(Stream stream) : base(stream) { }

        public IList<Box> Children { get; } = new List<Box>();

        public IEnumerator<Box> GetEnumerator() => Children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Children.GetEnumerator();
    }
}