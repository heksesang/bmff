using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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

        private IList<Box> _Children = Portability.CreateList<Box>();
        public IList<Box> Children
        {
            get { return _Children; }
        }

        public IEnumerator<Box> GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        /*
        public OriginalFormatBox OriginalFormat
        {
            get
            {
                return (from c in Children
                       where c is OriginalFormatBox
                       select (OriginalFormatBox)c).FirstOrDefault();
            }
        }
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
