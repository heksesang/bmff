using System.IO;
using System.Xml;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// XML Box ("xml ")
    /// </summary>
    [Box("xml ", "XML Box")]
    public sealed class XmlBox : Box, IContentBox
    {
        public XmlBox() : base() { }
        public XmlBox(Stream stream) : base(stream) { }

        public XmlReader GetXmlReader()
        {
            return XmlReader.Create(GetContentStream());
        }
    }
}
