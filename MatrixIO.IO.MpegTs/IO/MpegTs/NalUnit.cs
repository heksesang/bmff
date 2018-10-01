using System;

namespace MatrixIO.IO.MpegTs
{
    public class NalUnit
    {
        // 00 00 00 01
        private byte _header = 0x80;
        
        // 1 bit forbidden_zero_bit
        // 2 bit nal_ref_idc

        // 5 bit nal_unit_type

        public NalUnit(byte[] buffer, int offset, int length)
        {
            int position = offset;

            if (buffer[position++] != 0 || buffer[position++] != 0 || buffer[position++] != 1 || 
                (buffer[position] & 0x80) != 0x80)
                throw new ArgumentException("NAL Unit does not start with 0x000001 Start Code Prefix.");

            _header = buffer[position++];

            // TODO: Deserialize NAL Unit.
        }
    }
}
