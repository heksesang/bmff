using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatrixIO.IO.Bmff.Boxes.FairPlay
{
    [Box("drmi", "DRM Video Stream Box")]
    public class DrmVideoStreamBox : FullBox, ISuperBox
    {
        private IList<Box> _Children = Portability.CreateList<Box>();
        public IList<Box> Children
        {
            get
            {
                return _Children;
            }
        }

        public IEnumerator<Box> GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        public ProtectionSchemeInfoBox ProtectionSchemeInfo
        {
            get
            {
                return (from c in Children
                        where c is ProtectionSchemeInfoBox
                        select (ProtectionSchemeInfoBox)c).FirstOrDefault();
            }
        }
    }
}
