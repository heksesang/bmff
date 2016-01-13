using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// XML Box ("xml ")
    /// </summary>
    [Box("xml ", "XML Box")]
    public class XmlBox : Box, IContentBox
    {
        public XmlBox() : base() { }
        public XmlBox(Stream stream) : base(stream) { }

        public XmlReader GetXmlReader()
        {
            return XmlReader.Create(GetContentStream());
        }
    }
}
