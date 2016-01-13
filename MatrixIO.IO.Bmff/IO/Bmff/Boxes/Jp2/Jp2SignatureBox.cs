using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// JPEG2000 Signature Box
    /// </summary>
    [Box("jP  ", "JPEG2000 Signature Box")]
    public class Jp2SignatureBox : Box
    {
        public Jp2SignatureBox() : base() { }
        public Jp2SignatureBox(Stream stream) : base(stream) { }

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + (ulong)_Payload.Length;
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            _Payload = stream.ReadBytes((int)(EffectiveSize - 8));
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            stream.Write(_Payload, 0, _Payload.Length);
        }

        private byte[] _Payload = { 0x0D, 0x0A, 0x87, 0x0A }; // Default value of CR LF 0x87 LF as defined in 15444-1annexi
    }
}
