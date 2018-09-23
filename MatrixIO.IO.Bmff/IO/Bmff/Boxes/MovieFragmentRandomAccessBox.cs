using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Movie Fragment Random Access Box ("mfra")
    /// </summary>
    [Box("mfra", "Movie Fragment Random Access Box")]
    public class MovieFragmentRandomAccessBox : Box, ISuperBox
    {
        public MovieFragmentRandomAccessBox() : base() { }
        public MovieFragmentRandomAccessBox(Stream stream) : base(stream) { }

        public IList<Box> Children { get; } = new List<Box>();

        public IEnumerator<Box> GetEnumerator() => Children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Children.GetEnumerator();

        public IEnumerable<TrackFragmentRandomAccessBox> TrackFragmentRandomAccessBoxes
        {
            get => Children.OfType<TrackFragmentRandomAccessBox>();
        }

        public MovieFragmentRandomAccessOffsetBox MovieFragmentRandomAccessOffsetBox
        {
            get => Children.OfType<MovieFragmentRandomAccessOffsetBox>().LastOrDefault();
        }
    }
}