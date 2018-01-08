using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Resolution Box ("res ")
    /// </summary>
    [Box("res ", "Resolution Box")]
    public class ResolutionBox : Box, ISuperBox
    {
        public ResolutionBox() : base() { }
        public ResolutionBox(Stream stream) : base(stream) { }

        public IList<Box> Children { get; } = new List<Box>();

        public IEnumerator<Box> GetEnumerator() => Children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Children.GetEnumerator();
    }
}