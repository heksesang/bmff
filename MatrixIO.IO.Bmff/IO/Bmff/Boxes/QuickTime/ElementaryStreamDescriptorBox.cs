using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Elementary Stream Descriptor Atom ("esds")
    /// </summary>
    [Box("esds", "Elementary Stream Descriptor Atom")]
    public class ElementaryStreamDescriptorBox : FullBox, IContentBox
    {
        public ElementaryStreamDescriptorBox() : base() { }
        public ElementaryStreamDescriptorBox(Stream stream) : base(stream) { }

        // TODO: We should look in to parsing the contents of iods and esds which are specified in ISO 14496-1
    }
}
