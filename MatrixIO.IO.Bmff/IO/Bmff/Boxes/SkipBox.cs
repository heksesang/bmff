using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Skip Box ("skip")
    /// Alternative to the Free Space box.
    /// </summary>
    [Box("skip", "Skip Box")]
    public sealed class SkipBox : Box, IContentBox
    {
        public SkipBox() : base() { }
        public SkipBox(Stream stream) : base(stream) { }
    }
}
