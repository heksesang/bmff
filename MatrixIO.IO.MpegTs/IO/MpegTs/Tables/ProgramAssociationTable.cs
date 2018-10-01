namespace MatrixIO.IO.MpegTs.Tables
{
    public sealed class ProgramAssociationTable : TsTable<ProgramAssociation>
    {
        public ProgramAssociationTable() 
            : base() { }

        public ProgramAssociationTable(byte[] buffer, int offset, int length) : base(buffer, offset, length)
        {
            while (_position < _sectionLengthStartOffset + SectionLength - 4 && _position < offset + length - 4)
            {
                var row = new ProgramAssociation(buffer, _position);
                _position += row.Length;
                Rows.Add(row);
            }
        }

        public ushort Identifier
        {
            get => _identifier;
            set => _identifier = value;
        }
    }
}