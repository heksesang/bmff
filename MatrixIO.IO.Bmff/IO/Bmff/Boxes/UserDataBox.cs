using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        private IList<Box> _Children = Portability.CreateList<Box>();
        public IList<Box> Children
        {
            get { return _Children; }
        }

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + 4; // Fix for legacy trailing 0x0000000
        }

        protected override void SaveChildrenToStream(Stream stream)
        {
            base.SaveChildrenToStream(stream);
            stream.WriteBEUInt32(0);
        }

        public IEnumerator<Box> GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Children.GetEnumerator();
        }
    }
}
