using System.IO;
using System.Text;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Handler Box ("hdlr")
    /// </summary>
    // Don't ask me why the spec calls this Handler Reference Box but defines it as HandlerBox.  It's not corrected in the errata.
    [Box("hdlr", "Handler Reference Box")]
    public sealed class HandlerBox : FullBox
    {
        private uint[] _reserved = new uint[3];
        private uint _predefined;

        public HandlerBox()
            : base() { }

        public HandlerBox(Stream stream)
            : base(stream) { }

        public FourCC HandlerType { get; set; }

        public string Name { get; set; }

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + 4 + 4 + (4 * (ulong)_reserved.Length) + (string.IsNullOrEmpty(Name) ? 0 : (ulong)Encoding.UTF8.GetByteCount(Name));
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            _predefined = stream.ReadBEUInt32();
            HandlerType = new FourCC(stream.ReadBytes(4));

            for (int i = 0; i < _reserved.Length; i++)
            {
                _reserved[i] = stream.ReadBEUInt32();
            }

            Name = stream.ReadNullTerminatedUTF8String();
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            stream.WriteBEUInt32(_predefined);
            stream.WriteBEUInt32(HandlerType);

            for (int i = 0; i < _reserved.Length; i++)
            {
                stream.WriteBEUInt32(_reserved[i]);
            }

            if (Name != null)
            {
                stream.WriteUTF8String(Name);
            }
        }
    }
}