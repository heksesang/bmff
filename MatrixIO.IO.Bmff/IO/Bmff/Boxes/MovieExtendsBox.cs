using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Movie Extends Box ("mvex")
    /// </summary>
    [Box("mvex", "Movie Extends Box")]
    public class MovieExtendsBox : Box, ISuperBox
    {
        public MovieExtendsBox() : base() { }
        public MovieExtendsBox(Stream stream) : base(stream) { }

        public IList<Box> Children { get; } = new List<Box>();

        public IEnumerator<Box> GetEnumerator() => Children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Children.GetEnumerator();
    }
}