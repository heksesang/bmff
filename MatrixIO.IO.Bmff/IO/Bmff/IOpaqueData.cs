namespace MatrixIO.IO.Bmff
{
    public interface IOpaqueData
    {
        long DataOffset { get; }
        long DataLength { get; }
    }
}