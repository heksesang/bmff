using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Track Fragment Box ("traf")
    /// </summary>
    [Box("traf", "Track Fragment Box")]
    public sealed class TrackFragmentBox : Box, ISuperBox
    {
        public TrackFragmentBox() : base() { }
        public TrackFragmentBox(Stream stream) : base(stream) { }

        public IList<Box> Children { get; } = new List<Box>();

        public IEnumerator<Box> GetEnumerator() => Children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Children.GetEnumerator();
    }
}