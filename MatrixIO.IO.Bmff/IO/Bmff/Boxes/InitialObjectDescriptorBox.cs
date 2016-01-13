using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Initial Object Descriptor Box ("iods")
    /// </summary>
    [Box("iods", "Initial Object Descriptor Box")]
    public class InitialObjectDescriptorBox : FullBox, IContentBox
    {
        public InitialObjectDescriptorBox() : base() { }
        public InitialObjectDescriptorBox(Stream stream) : base(stream) { }

        // TODO: We should look in to parsing the contents of iods and esds which are specified in ISO 14496-1
    }
}
