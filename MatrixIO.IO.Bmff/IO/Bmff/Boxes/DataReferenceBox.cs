using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Data Reference Box ("dref")
    /// </summary>
    [Box("dref", "Data Reference Box")]
    public class DataReferenceBox : FullBox, ISuperBox
    {
        public DataReferenceBox() : base() { }
        public DataReferenceBox(Stream stream) : base(stream) { }

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + 4;
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            _EntryCount = stream.ReadBEUInt32();
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            stream.WriteBEUInt32((uint)_Children.Count);
        }

        protected override void LoadChildrenFromStream(Stream stream)
        {
            base.LoadChildrenFromStream(stream);

            if (_EntryCount != _Children.Count) Trace.WriteLine("DataReferenceBox's EntryCount didn't match the number of children we read.", "WARNING");
        }

        private uint _EntryCount;

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
    }
}
