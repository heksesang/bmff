using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Media Box ("mdia")
    /// A common base structure is used to contain general metadata, called the meta box.
    /// </summary>
    [Box("mdia", "Media Box")]
    public class MediaBox : Box, ISuperBox
    {
        public MediaBox() : base() { }
        public MediaBox(Stream stream) : base(stream) { }

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
