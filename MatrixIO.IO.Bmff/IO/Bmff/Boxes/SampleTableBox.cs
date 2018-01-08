using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Sample Table Box ("stbl")
    /// </summary>
    [Box("stbl", "Sample Table Box")]
    public class SampleTableBox : Box, ISuperBox
    {
        public SampleTableBox() : base() { }
        public SampleTableBox(Stream stream) : base(stream) { }

        public IList<Box> Children { get; } = new List<Box>();

        public IEnumerator<Box> GetEnumerator() => Children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Children.GetEnumerator();
    }
}