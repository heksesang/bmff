using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatrixIO.IO.Ebml.Elements
{
    [Element(0x42, 0x86, ElementTypes.UnsignedInteger, "The version of EBML parser used to create the file.")]
    public class EbmlVersionElement : Element
    {

    }
}
