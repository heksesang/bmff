using System.Collections.Generic;
using System.Linq;
using MatrixIO.IO.MpegTs.Descriptors;

namespace MatrixIO.IO.MpegTs.Tables
{
    public sealed class ProgramMapTable : TsTable<ProgramMap>
    {
        private ushort _programClockReferencePid;
        private ushort _programInfoLength;

        public ProgramMapTable() { }
        public ProgramMapTable(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
            _programClockReferencePid = (ushort)((buffer[_position++] << 8) | buffer[_position++]);
            _programInfoLength = (ushort)((buffer[_position++] << 8) | buffer[_position++]);

            ProgramInfo = new List<TsDescriptor>();
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

        public ushort ProgramNumber
        {
            get => _identifier;
            set => _identifier = value;
        }

        public ushort ProgramClockReferencePid
        {
            get => (ushort)(_programClockReferencePid & 0x1FFF);
            set => _programClockReferencePid = (ushort)((_programClockReferencePid & 0xE000) | (value & 0x1FFF));
        }

        public ushort ProgramInfoLength
        {
            get => (ushort)(_programInfoLength & 0x0FFF);
            set => _programInfoLength = (ushort)((_programInfoLength & 0xF000) | (value & 0x0FFF));
        }

        public IList<TsDescriptor> ProgramInfo { get; private set; }
    }

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