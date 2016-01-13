using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatrixIO.IO.Ebml
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class ElementAttribute : Attribute
    {
        public ClassIdentifier ClassIdentifier { get; private set; }
        public ElementTypes Type { get; private set; }
        public string Description { get; private set; }

        private ElementAttribute(ElementTypes type, string description = null)
        {
            Type = type;
            Description = description;
        }

        public ElementAttribute(int identifier, ElementTypes type, string description = null) : this(type, description)
        {
            ClassIdentifier = new ClassIdentifier(identifier);
        }

        public ElementAttribute(byte a, ElementTypes type, string description = null) : this(type, description)
        {
            ClassIdentifier = new ClassIdentifier(a);
        }
        public ElementAttribute(byte a, byte b, ElementTypes type, string description = null) : this(type, description)
        {
            ClassIdentifier = new ClassIdentifier(a, b);
        }
        public ElementAttribute(byte a, byte b, byte c, ElementTypes type, string description = null) : this(type, description)
        {
            ClassIdentifier = new ClassIdentifier(a, b, c);
        }
        public ElementAttribute(byte a, byte b, byte c, byte d, ElementTypes type, string description = null) : this(type, description)
        {
            ClassIdentifier = new ClassIdentifier(a, b, c, d);
        }
        public ElementAttribute(ClassIdentifier identifier, ElementTypes type, string description = null) : this(type, description)
        {
            ClassIdentifier = identifier;
        }
    }

}
