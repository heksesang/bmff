using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Track Aperture Mode Dimensions Atom ("tapt")
    /// </summary>
    [Box("tapt", "Track Aperture Mode Dimensions Atom")]
    public class TrackApertureModeDimensionsBox : Box, ISuperBox
    {
        public TrackApertureModeDimensionsBox() : base() { }
        public TrackApertureModeDimensionsBox(Stream stream) : base(stream) { }

        protected IList<Box> _Children = Portability.CreateList<Box>();
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
