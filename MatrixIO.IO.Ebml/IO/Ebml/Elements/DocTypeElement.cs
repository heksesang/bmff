using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatrixIO.IO.Ebml.Elements
{
    [Element(0x42, 0x82, ElementTypes.String, "A string that describes the type of document that follows this EBML header ('matroska' in our case).")]
    public class DocTypeElement
    {
    }
}
