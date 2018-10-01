using System.Collections.Generic;
using System.Linq;
using MatrixIO.IO.MpegTs.Descriptors;

namespace MatrixIO.IO.MpegTs.Tables
{
    public class ProgramMap
    {
        public ProgramMap() { }

        public ProgramMap(byte[] buffer, int offset)
        {
            var position = offset;
            StreamType = (StreamType)buffer[position++];
            PacketIdentifier = (ushort)(((buffer[position++] << 8) | buffer[position++]) & 0x1FFF);
            var streamInfoLength = (ushort)(((buffer[position++] << 8) | buffer[position++]) & 0x0FFF);

            StreamInfo = new List<TsDescriptor>();

            var descriptorEndPosition = position + streamInfoLength;

            while (position < descriptorEndPosition)
            {
                var descriptor = new UnknownDescriptor(buffer, position);
                StreamInfo.Add(descriptor);
                position += descriptor.Length;
            }
        }

        public int Length => 1 + 2 + 2 + StreamInfo.Sum(d => d.Length);

        public StreamType StreamType { get; set; }

        public ushort PacketIdentifier { get; set; }

        public IList<TsDescriptor> StreamInfo { get; set; }
    }
}