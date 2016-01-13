using System;
using System.Collections.Generic;
using System.IO;
using MatrixIO.IO.Ebml.Elements;

namespace MatrixIO.IO.Ebml
{
    public abstract class Element
    {
        public long Length { get; set; }

        protected Element() {}
        protected Element(Stream source)
        {
            var identifier = new ClassIdentifier(source);
            Initialize(source);
        }

        private void Initialize(Stream source)
        {
            
        }

        private static readonly Dictionary<ClassIdentifier, Type> ElementDefinitions = new Dictionary<ClassIdentifier, Type>();

        static Element FromStream(Stream source)
        {
            var identifier = new ClassIdentifier(source);
            var elementType = ElementDefinitions[identifier];
            Element element = elementType != null ? (Element)Activator.CreateInstance(elementType) : new UnknownElement(identifier);
            element.Initialize(source);
            return element;
        }
    }
}
