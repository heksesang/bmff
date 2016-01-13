using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatrixIO.IO.MpegTs
{
    public enum AACSCopyControl
    {
        // TODO: Figure out what these mean from the AACS spec
        Unencrypted,
        Unknown1,
        Unknown2,
        Unknown3,
    }

    public class AacsPacket
    {
        public AACSCopyControl CopyControl { get; private set; }

        /// <summary>27Mhz Clock</summary>
        public uint Timestamp { get; private set; }

        public TsPacket TsPacket { get; private set; }

        public const int Length = 4+188;

        public AacsPacket(byte[] buffer, int offset)
        {
            CopyControl = (AACSCopyControl)((buffer[offset] & 0xC0) >> 6);
            Timestamp = (uint)(((buffer[offset++] << 24) & 0x3F) | (buffer[offset++] << 16) | (buffer[offset++] << 8) | buffer[offset++]);

            if (CopyControl == AACSCopyControl.Unencrypted)
            {
                TsPacket = new TsPacket(buffer, offset);
            }
            else
            {
                throw new FormatException("Encrypted content not currently supported.");
            }
        }
    }
}
