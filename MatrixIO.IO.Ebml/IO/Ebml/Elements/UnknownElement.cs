using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatrixIO.IO.Ebml.Elements
{
    public class UnknownElement : Element
    {
        public ClassIdentifier Identifier { get; private set; }

        public UnknownElement(ClassIdentifier identifier)
        {
            Identifier = identifier;
        }
    }
}
