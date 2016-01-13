using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
