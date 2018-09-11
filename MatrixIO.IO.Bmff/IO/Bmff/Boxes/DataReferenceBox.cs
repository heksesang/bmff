using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Data Reference Box ("dref")
    /// </summary>
    [Box("dref", "Data Reference Box")]
    public sealed class DataReferenceBox : FullBox, ISuperBox
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

            stream.WriteBEUInt32((uint)Children.Count);
        }

        protected override void LoadChildrenFromStream(Stream stream)
        {
            base.LoadChildrenFromStream(stream);

            if (_EntryCount != Children.Count)
            {
                Trace.WriteLine("DataReferenceBox's EntryCount didn't match the number of children we read.", "WARNING");
            }
        }

        private uint _EntryCount;

        public IList<Box> Children { get; } = new List<Box>();

        public IEnumerator<Box> GetEnumerator() => Children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Children.GetEnumerator();
    }
}