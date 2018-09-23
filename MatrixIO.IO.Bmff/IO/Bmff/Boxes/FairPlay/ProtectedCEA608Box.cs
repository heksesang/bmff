using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MatrixIO.IO.Bmff.Boxes.FairPlay
{
    [Box("p608", "Protected CEA-608 Box")]
    public sealed class ProtectedCEA608Box : FullBox, ISuperBox
    {
        public IList<Box> Children { get; } = new List<Box>();

        public IEnumerator<Box> GetEnumerator() => Children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Children.GetEnumerator();

        public ProtectionSchemeInfoBox ProtectionSchemeInfo
        {
            get => Children.OfType<ProtectionSchemeInfoBox>().FirstOrDefault();
        }
    }
}