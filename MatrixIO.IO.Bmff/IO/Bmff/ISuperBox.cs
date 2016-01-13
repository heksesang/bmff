using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MatrixIO.IO.Bmff
{
    /// <summary>
    /// Identifies a Container (BMFF) or SuperBox (JPEG200) that contains child Boxes.
    /// </summary>
    public interface ISuperBox : IEnumerable<Box>
    {
        IList<Box> Children { get; }
    }
}
