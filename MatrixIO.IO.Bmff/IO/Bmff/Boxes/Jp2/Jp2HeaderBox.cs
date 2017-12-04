using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// JPEG2000 Header Box ("jp2h")
    /// </summary>
    [Box("jp2h", "JPEG2000 Header Box")]
    public class Jp2HeaderBox: Box, ISuperBox
    {
        public Jp2HeaderBox() : base() { }
        public Jp2HeaderBox(Stream stream) : base(stream) { }

        public IList<Box> Children { get; } = new List<Box>();

        public IEnumerator<Box> GetEnumerator() => Children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Children.GetEnumerator();
    }
}