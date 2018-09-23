using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Protection Scheme Info Box ("sinf")
    /// </summary>
    [Box("sinf", "Protection Scheme Info Box")]
    public sealed class ProtectionSchemeInfoBox : Box, ISuperBox
    {
        public ProtectionSchemeInfoBox() : base() { }
        public ProtectionSchemeInfoBox(Stream stream) : base(stream) { }

        public IList<Box> Children { get; } = new List<Box>();

        public IEnumerator<Box> GetEnumerator() => Children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Children.GetEnumerator();

        public OriginalFormatBox OriginalFormat
        {
            get => Children.OfType<OriginalFormatBox>().FirstOrDefault();
        }

        /*
        public SchemeTypeBox SchemeType
        {
            get => Children.OfType<SchemeTypeBox>().FirstOrDefault();
        }

        public SchemeInformationBox Info
        {
            get => Children.OfType<SchemeInformationBox>().FirstOrDefault();
        }
        */
    }
}