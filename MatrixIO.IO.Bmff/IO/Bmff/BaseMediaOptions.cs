using System;

namespace MatrixIO.IO.Bmff
{
    [Flags]
    public enum BaseMediaOptions
    {
        None,
        CacheAllBoxes,
        CacheAllBoxContent,
        LoadChildren,
    }
}
