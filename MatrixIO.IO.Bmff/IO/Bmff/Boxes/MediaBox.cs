using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Media Box ("mdia")
    /// A common base structure is used to contain general metadata, called the meta box.
    /// </summary>
    [Box("mdia", "Media Box")]
    public class MediaBox : Box, ISuperBox
    {
        public MediaBox() : base() { }
        public MediaBox(Stream stream) : base(stream) { }

        public IList<Box> Children { get; } = new List<Box>();

        public IEnumerator<Box> GetEnumerator() => Children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Children.GetEnumerator();
    }
}