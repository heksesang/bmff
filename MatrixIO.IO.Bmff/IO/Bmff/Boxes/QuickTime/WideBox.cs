using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// QuickTime Wide Atom ("wide")
    /// </summary>
    [Box("wide", "Wide Atom")]
    public class WideBox : Box, IContentBox
    {
        public WideBox() : base() { }
        public WideBox(Stream stream) : base(stream) { }
    }
}
