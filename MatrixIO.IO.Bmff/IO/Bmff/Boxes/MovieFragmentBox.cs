using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Movie Fragment Box ("moof")
    /// </summary>
    [Box("moof", "Movie Fragment Box")]
    public class MovieFragmentBox : Box, ISuperBox
    {
        public MovieFragmentBox() : base() { }
        public MovieFragmentBox(Stream stream) : base(stream) { }

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
