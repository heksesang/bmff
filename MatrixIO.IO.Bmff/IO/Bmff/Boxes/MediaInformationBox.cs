using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Media Information Box ("minf")
    /// </summary>
    [Box("minf", "Media Information Box")]
    public sealed class MediaInformationBox : Box, ISuperBox
    {
        public MediaInformationBox() : base() { }
        public MediaInformationBox(Stream stream) : base(stream) { }

        public IList<Box> Children { get; } = new List<Box>();

        public IEnumerator<Box> GetEnumerator() => Children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Children.GetEnumerator();
    }
}