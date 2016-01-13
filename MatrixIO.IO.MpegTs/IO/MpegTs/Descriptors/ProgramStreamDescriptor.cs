using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatrixIO.IO.MpegTs.Descriptors
{
    public class ProgramStreamDescriptor : TsDescriptor
    {
        private byte _flags1;
        private byte _flags2;

        public ProgramStreamDescriptor(byte[] buffer, int offset) : base(buffer, offset)
        {
            _flags1 = buffer[_position++];
            _flags2 = buffer[_position++];

            // TODO: Load Program Stream Descriptor
        }
    }
}
