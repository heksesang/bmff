using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Partition Entry Box ("paen")
    /// </summary>
    [Box("paen", "Partition Entry Box")]
    public sealed class PartitionEntryBox : Box, ISuperBox
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
             get => Children.OfType<FilePartitionBox>().FirstOrDefault();
        }

        public FECReservoirBox FECSymbolLocations
        {
             get => Children.OfType<FilePartitionBox>().FirstOrDefault();

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
