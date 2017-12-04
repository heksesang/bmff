using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// User Data Box ("udta")
    /// </summary>
    [Box("udta", "User Data Box")]
    public class UserDataBox : Box, ISuperBox
    {
        public UserDataBox() : base() { }
        public UserDataBox(Stream stream) : base(stream) { }

        public IList<Box> Children { get; } = new List<Box>();

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + 4; // Fix for legacy trailing 0x0000000
        }

        protected override void SaveChildrenToStream(Stream stream)
        {
            base.SaveChildrenToStream(stream);
            stream.WriteBEUInt32(0);
        }

        public IEnumerator<Box> GetEnumerator() => Children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Children.GetEnumerator();
    }
}