using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Additional Metadata Container Box ("meco")
    /// </summary>
    [Box("meco", "Additional Metadata Container Box")]
    public sealed class AdditionalMetadataContainerBox : Box, ISuperBox
    {
        public AdditionalMetadataContainerBox() : base() { }
        public AdditionalMetadataContainerBox(Stream stream) : base(stream) { }

        public IList<Box> Children { get; } = new List<Box>();

        public IEnumerator<Box> GetEnumerator() => Children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Children.GetEnumerator();
    }
}