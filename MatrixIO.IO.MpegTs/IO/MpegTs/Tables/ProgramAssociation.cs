namespace MatrixIO.IO.MpegTs.Tables
{
    public readonly struct ProgramAssociation
    {
        public ProgramAssociation(byte[] buffer, int offset)
        {
            int position = offset;
            ProgramNumber = (ushort)((buffer[position++] << 8) | buffer[position++]);
            PacketIdentifier = (ushort)(((buffer[position++] << 8) | buffer[position]) & 0x1FFF);
        }

        public int Length => 2 + 2;

        public ushort ProgramNumber { get; }

        public ushort PacketIdentifier { get; }

    }
}