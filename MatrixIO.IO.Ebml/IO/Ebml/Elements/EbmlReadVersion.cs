using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatrixIO.IO.Ebml.Elements
{
    [Element(0x42, 0xF7, ElementTypes.UnsignedInteger, "The minimum EBML version a parser has to support to read this file.")]
    public class EbmlReadVersion
    {
    }
}
