using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Null Media Header Box ("nmhd")
    /// </summary>
    [Box("nmhd", "Null Media Header Box")]
    public sealed class NullMediaHeaderBox : FullBox
    {
        public NullMediaHeaderBox() : base() { }
        public NullMediaHeaderBox(Stream stream) : base(stream) { }
    }
}
