using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Movie Fragment Box ("moof")
    /// </summary>
    [Box("moof", "Movie Fragment Box")]
    public sealed class MovieFragmentBox : Box, ISuperBox
    {
        public MovieFragmentBox() : base() { }
        public MovieFragmentBox(Stream stream) : base(stream) { }

        public IList<Box> Children { get; } = new List<Box>();

        public IEnumerator<Box> GetEnumerator() => Children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Children.GetEnumerator();
    }
}