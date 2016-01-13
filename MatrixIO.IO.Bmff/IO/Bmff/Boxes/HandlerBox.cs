using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Handler Box ("hdlr")
    /// </summary>
    // Don't ask me why the spec calls this Handler Reference Box but defines it as HandlerBox.  It's not corrected in the errata.
    [Box("hdlr", "Handler Reference Box")]
    public class HandlerBox : FullBox
    {
        public HandlerBox() : base() { }
        public HandlerBox(Stream stream) : base(stream) { }

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + 4 + 4 + (4 * (ulong)_Reserved.Length) + (String.IsNullOrEmpty(Name) ? 0 : (ulong)Encoding.UTF8.GetByteCount(Name));
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            _Predefined = stream.ReadBEUInt32();
            HandlerType = new FourCC(stream.ReadBytes(4));
            for (int i = 0; i < _Reserved.Length; i++) _Reserved[i] = stream.ReadBEUInt32();
            Name = stream.ReadNullTerminatedUTF8String();
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            stream.WriteBEUInt32(_Predefined);
            stream.WriteBEUInt32(HandlerType);
            for (int i = 0; i < _Reserved.Length; i++) stream.WriteBEUInt32(_Reserved[i]);
            if(Name != null)
                stream.WriteUTF8String(Name);
        }

        private uint _Predefined;
        public FourCC HandlerType { get; set; }

        private uint[] _Reserved = new uint[3];

        public string Name { get; set; }
    }
}
