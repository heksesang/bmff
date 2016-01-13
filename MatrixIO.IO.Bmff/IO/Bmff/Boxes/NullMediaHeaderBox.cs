using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Null Media Header Box ("nmhd")
    /// </summary>
    [Box("nmhd", "Null Media Header Box")]
    public class NullMediaHeaderBox : FullBox
    {
        public NullMediaHeaderBox() : base() { }
        public NullMediaHeaderBox(Stream stream) : base(stream) { }
    }
}
