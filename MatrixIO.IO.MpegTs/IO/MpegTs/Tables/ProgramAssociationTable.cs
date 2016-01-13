using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatrixIO.IO.MpegTs.Tables
{
    public class ProgramAssociationTable : TsTable<ProgramAssociation>
    {
        public ushort Identifier { get { return _identifier; } set { _identifier = value; } }

        public ProgramAssociationTable() : base() {}
        public ProgramAssociationTable(byte[] buffer, int offset, int length) : base(buffer, offset, length)
        {
            while (_position < _sectionLengthStartOffset + SectionLength - 4 && _position < offset + length - 4)
            {
                var row = new ProgramAssociation(buffer, _position);
                _position += row.Length;
                Rows.Add(row);
            }
        }
    }

    public class ProgramAssociation
    {
        public int Length { get { return 2 + 2; } }
        public ushort ProgramNumber { get; set; }
        public ushort PacketIdentifier { get; set; }

        public ProgramAssociation() { }
        public ProgramAssociation(byte[] buffer, int offset)
        {
            int position = offset;
            ProgramNumber = (ushort) ((buffer[position++] << 8) | buffer[position++]);
            PacketIdentifier = (ushort) (((buffer[position++] << 8) | buffer[position]) & 0x1FFF);
        }
    }
}
