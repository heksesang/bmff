using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// JPEG2000 Codestream Box ("jp2c")
    /// </summary>
    [Box("jp2c", "Contiguous Codestream Box")]
    public class ContiguousCodestreamBox: Box
    {
        public ContiguousCodestreamBox() : base() { }
        public ContiguousCodestreamBox(Stream stream) : base(stream) { }
    }
}
