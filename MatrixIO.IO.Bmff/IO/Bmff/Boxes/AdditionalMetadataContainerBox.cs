using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Additional Metadata Container Box ("meco")
    /// </summary>
    [Box("meco", "Additional Metadata Container Box")]
    public class AdditionalMetadataContainerBox : Box, ISuperBox
    {
        public AdditionalMetadataContainerBox() : base() { }
        public AdditionalMetadataContainerBox(Stream stream) : base(stream) { }

        private readonly IList<Box> _children = Portability.CreateList<Box>();
        public IList<Box> Children
        {
            get { return _children; }
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
