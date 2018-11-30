using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Data Information Box ("dinf")
    /// </summary>
    [Box("dinf", "Data Information Box")]
    public sealed class DataInformationBox : Box, ISuperBox
    {
        public DataInformationBox() 
            : base() { }

        public DataInformationBox(Stream stream) 
            : base(stream) { }

        public IList<Box> Children { get; } = new List<Box>();

        public IEnumerator<Box> GetEnumerator() => Children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Children.GetEnumerator();
    }
}