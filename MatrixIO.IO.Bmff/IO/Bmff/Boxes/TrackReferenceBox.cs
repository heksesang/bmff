using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Track Reference Box ("tref")
    /// </summary>
    [Box("tref", "Track Reference Box")]
    public class TrackReferenceBox : Box, ISuperBox
    {
        public TrackReferenceBox() : base() { }
        public TrackReferenceBox(Stream stream) : base(stream) { }

        public int EntryCount { get { return _Children.Count; } }

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
