using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Free Space Box ("free")
    /// </summary>
    [Box("free", "Free Space Box")]
    public sealed class FreeSpaceBox : Box, IContentBox
    {
        public FreeSpaceBox() : base() { }
        public FreeSpaceBox(ulong size) : base()
        {
            this.Offset = 0;
            this.EffectiveSize = size;
            this._sourceStream = Stream.Null;
            ContentOffset = this.CalculateSize();
        }
        public FreeSpaceBox(Stream stream) : base(stream) { }
    }
}
