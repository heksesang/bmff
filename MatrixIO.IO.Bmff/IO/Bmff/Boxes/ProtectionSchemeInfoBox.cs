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
    public class ProtectionSchemeInfoBox : Box, ISuperBox
    {
        public ProtectionSchemeInfoBox() : base() { }
        public ProtectionSchemeInfoBox(Stream stream) : base(stream) { }

        public IList<Box> Children { get; } = new List<Box>();

        public IEnumerator<Box> GetEnumerator() => Children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()=> Children.GetEnumerator();

        public OriginalFormatBox OriginalFormat
        {
            get
            {
                return (from c in Children
                       where c is OriginalFormatBox
                       select (OriginalFormatBox)c).FirstOrDefault();
            }
        }

        /*
        public SchemeTypeBox SchemeType
        {
            get
            {
                return (from c in Children
                        where c is SchemeTypeBox
                        select (SchemeTypeBox)c).FirstOrDefault();
            }
        }
        public SchemeInformationBox Info
        {
            get
            {
                return (from c in Children
                        where c is SchemeInformationBox
                        select (SchemeInformationBox)c).FirstOrDefault();
            }
        }
        */
    }
}