using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MatrixIO.IO.Ebml.Elements
{
    [Element(0x1A, 0x45, 0xDF, 0xA3, ElementTypes.MasterElement, "Set the EBML characteristics of the data to follow. Each EBML document has to start with this.")]
    public class EbmlElement : Element
    {
        public EbmlElement(Stream source) : base(source)
        {
            
        }
    }
}
