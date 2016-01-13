using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatrixIO.IO.Ebml.Elements
{
    [Element(0x42, 0xF2, ElementTypes.UnsignedInteger, "The maximum length of the IDs you'll find in this file (4 or less in Matroska).")]
    public class EbmlMaxIdentifierLength
    {
    }
}
