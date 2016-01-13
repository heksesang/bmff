using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatrixIO.IO.Ebml.Elements
{
    [Element(0x42, 0xF3, ElementTypes.UnsignedInteger, "The maximum length of the sizes you'll find in this file (8 or less in Matroska). This does not override the element size indicated at the beginning of an element. Elements that have an indicated size which is larger than what is allowed by EBMLMaxSizeLength shall be considered invalid.")]
    public class EbmlMaxSizeLength
    {

    }
}
