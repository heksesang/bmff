using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Hint Media Header Box ("hmhd")
    /// </summary>
    [Box("hmhd", "Hint Media Header Box")]
    public class HintMediaHeaderBox : FullBox
    {
        public HintMediaHeaderBox() : base() { }
        public HintMediaHeaderBox(Stream stream) : base(stream) { }

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + 2 + 2 + 4 + 4 + 4;
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            MaxProtocolDataUnitSize = stream.ReadBEUInt16();
            AverageProtocolDataUnitSize = stream.ReadBEUInt16();
            MaxBitrate = stream.ReadBEUInt32();
            AverageBitrate = stream.ReadBEUInt32();
            _Reserved = stream.ReadBEUInt32();
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            stream.WriteBEUInt16(MaxProtocolDataUnitSize);
            stream.WriteBEUInt16(AverageProtocolDataUnitSize);
            stream.WriteBEUInt32(MaxBitrate);
            stream.WriteBEUInt32(AverageBitrate);
            stream.WriteBEUInt32(_Reserved);
        }

        public ushort MaxProtocolDataUnitSize { get; set; }
        public ushort AverageProtocolDataUnitSize { get; set; }
        public uint MaxBitrate { get; set; }
        public uint AverageBitrate { get; set; }
        private uint _Reserved;
    }
}
