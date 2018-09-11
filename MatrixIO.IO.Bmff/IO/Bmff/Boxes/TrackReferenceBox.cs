using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Track Reference Box ("tref")
    /// </summary>
    [Box("tref", "Track Reference Box")]
    public sealed class TrackReferenceBox : Box, ISuperBox
    {
        public TrackReferenceBox() : base() { }
        public TrackReferenceBox(Stream stream) : base(stream) { }

        public IList<Box> Children { get; } = new List<Box>();

        public int EntryCount => Children.Count;

        public IEnumerator<Box> GetEnumerator() => Children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Children.GetEnumerator();
    }
}