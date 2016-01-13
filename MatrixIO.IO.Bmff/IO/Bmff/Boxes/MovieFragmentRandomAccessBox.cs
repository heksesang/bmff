using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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

        private IList<Box> _Children = Portability.CreateList<Box>();
        public IList<Box> Children
        {
            get { return _Children; }
        }

        public IEnumerator<Box> GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        public IEnumerable<TrackFragmentRandomAccessBox> TrackFragmentRandomAccessBoxes
        {
            get
            {
                return from c in Children
                        where c is TrackFragmentRandomAccessBox
                        select (TrackFragmentRandomAccessBox)c;
            }
        }

        public MovieFragmentRandomAccessOffsetBox MovieFragmentRandomAccessOffsetBox
        {
            get
            {
                return (from c in Children
                        where c is MovieFragmentRandomAccessOffsetBox
                        select (MovieFragmentRandomAccessOffsetBox) c).LastOrDefault();
            }
        }


    }
}
