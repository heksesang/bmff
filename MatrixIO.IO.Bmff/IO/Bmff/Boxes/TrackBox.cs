using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Track Box ("trak")
    /// </summary>
    [Box("trak", "Track Box")]
    public sealed class TrackBox : Box, ISuperBox
    {
        public TrackBox() : base() { }
        public TrackBox(Stream stream) : base(stream) { }

        public IList<Box> Children { get; } = new List<Box>();

        public IEnumerator<Box> GetEnumerator() => Children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Children.GetEnumerator();
    }
}