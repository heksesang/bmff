using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Partition Entry Box ("paen")
    /// </summary>
    [Box("paen", "Partition Entry Box")]
    public class PartitionEntryBox : Box, ISuperBox
    {
        public PartitionEntryBox() : base() { }
        public PartitionEntryBox(Stream stream) : base(stream) { }

        public IList<Box> Children { get; } = new List<Box>();

        public IEnumerator<Box> GetEnumerator() => Children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Children.GetEnumerator();

        // TODO: Standard Boxes
        /*
        public FilePartitionBox BlocksAndSymbols
        {
            get
            {
                return (from c in Children
                        where c is FilePartitionBox
                        select (FilePartitionBox)c).FirstOrDefault();
            }
        }

        public FECReservoirBox FECSymbolLocations
        {
            get
            {
                return (from c in Children
                        where c is FECReservoirBox
                        select (FECReservoirBox)c).FirstOrDefault();
            }
        }
        */
    }
}
