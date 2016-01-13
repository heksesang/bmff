using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Track Fragment Box ("traf")
    /// </summary>
    [Box("traf", "Track Fragment Box")]
    public class TrackFragmentBox : Box, ISuperBox
    {
        public TrackFragmentBox() : base() { }
        public TrackFragmentBox(Stream stream) : base(stream) { }

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
    }
}
