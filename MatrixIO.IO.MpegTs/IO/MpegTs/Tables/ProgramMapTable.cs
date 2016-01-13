using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MatrixIO.IO.MpegTs.Descriptors;

namespace MatrixIO.IO.MpegTs.Tables
{
    public class ProgramMapTable : TsTable<ProgramMap>
    {
        public ushort ProgramNumber { get { return _identifier; } set { _identifier = value; } }

        private ushort _programClockReferencePid;
        public ushort ProgramClockReferencePid
        {
            get { return (ushort)(_programClockReferencePid & 0x1FFF); }
            set { _programClockReferencePid = (ushort)((_programClockReferencePid & 0xE000) | (value & 0x1FFF)); }
        }

        private ushort _programInfoLength;
        public ushort ProgramInfoLength
        {
            get { return (ushort)(_programInfoLength & 0x0FFF); }
            set { _programInfoLength = (ushort)((_programInfoLength & 0xF000) | (value & 0x0FFF)); }
        }

        public IList<TsDescriptor> ProgramInfo { get; private set; }

        public ProgramMapTable() { }
        public ProgramMapTable(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
            _programClockReferencePid = (ushort)((buffer[_position++] << 8) | buffer[_position++]);
            _programInfoLength = (ushort)((buffer[_position++] << 8) | buffer[_position++]);

            ProgramInfo = Portability.CreateList<TsDescriptor>();
            var descriptorEndPosition = _position + ProgramInfoLength;
            while (_position < descriptorEndPosition)
            {
                var descriptor = new UnknownDescriptor(buffer, _position);
                ProgramInfo.Add(descriptor);
                _position += descriptor.Length;
            }

            while (_position < _sectionLengthStartOffset + SectionLength - 4 && _position < offset + length - 4)
            {
                var row = new ProgramMap(buffer, _position);
                _position += row.Length;
                Rows.Add(row);
            }
        }
    }

    public class ProgramMap
    {
        public int Length { get { return 1 + 2 + 2 + StreamInfo.Sum(d => d.Length); } }
        public StreamType StreamType { get; set; }
        public ushort PacketIdentifier { get; set; }
        public IList<TsDescriptor> StreamInfo { get; set; }

        public ProgramMap() { }
        public ProgramMap(byte[] buffer, int offset)
        {
            var position = offset;
            StreamType = (StreamType) buffer[position++];
            PacketIdentifier = (ushort) (((buffer[position++] << 8) | buffer[position++]) & 0x1FFF);
            var streamInfoLength = (ushort)(((buffer[position++] << 8) | buffer[position++]) & 0x0FFF);

            StreamInfo = Portability.CreateList<TsDescriptor>();
            var descriptorEndPosition = position + streamInfoLength;
            while (position < descriptorEndPosition)
            {
                var descriptor = new UnknownDescriptor(buffer, position);
                StreamInfo.Add(descriptor);
                position += descriptor.Length;
            }
        }
    }
}