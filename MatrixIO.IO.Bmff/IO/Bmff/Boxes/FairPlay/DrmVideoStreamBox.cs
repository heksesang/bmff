using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MatrixIO.IO.Bmff.Boxes.FairPlay
{
    [Box("drmi", "DRM Video Stream Box")]
    public sealed class DrmVideoStreamBox : FullBox, ISuperBox
    {
        public IList<Box> Children { get; } = new List<Box>();

        public IEnumerator<Box> GetEnumerator() => Children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Children.GetEnumerator();

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