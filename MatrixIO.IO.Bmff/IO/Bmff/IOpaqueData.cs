using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatrixIO.IO.Bmff
{
    public interface IOpaqueData
    {
        long DataOffset { get; }
        long DataLength { get; }

        
    }
}
