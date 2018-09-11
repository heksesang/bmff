using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Track Aperture Mode Dimensions Atom ("tapt")
    /// </summary>
    [Box("tapt", "Track Aperture Mode Dimensions Atom")]
    public sealed class TrackApertureModeDimensionsBox : Box, ISuperBox
    {
        public TrackApertureModeDimensionsBox() : base() { }
        public TrackApertureModeDimensionsBox(Stream stream) : base(stream) { }

        public IList<Box> Children { get; } = new List<Box>();

        public IEnumerator<Box> GetEnumerator() => Children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Children.GetEnumerator();
    }
}