namespace MatrixIO.IO.MpegTs.Streams
{
    public sealed class ByteStream : TsStream<byte[]>
    {
        protected override byte[] ProcessUnit(TsUnit unit)
        {
            // TODO: We should think about passing the unit payload in as an IList for speed.  It complicates copying chunks of data, however.
            var unitPayload = new byte[unit.Payload.Count];
            unit.Payload.CopyTo(unitPayload, 0);
            return unitPayload;
        }
    }
}