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
        private uint _entryCount;

        public DataReferenceBox()
            : base() { }

        public DataReferenceBox(Stream stream)
            : base(stream) { }

        public IList<Box> Children { get; } = new List<Box>();

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + 4;
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            _entryCount = stream.ReadBEUInt32();
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            stream.WriteBEUInt32((uint)Children.Count);
        }

        protected override void LoadChildrenFromStream(Stream stream)
        {
            base.LoadChildrenFromStream(stream);

            if (_entryCount != Children.Count)
            {
                Trace.WriteLine("DataReferenceBox's EntryCount didn't match the number of children we read.", "WARNING");
            }
        }

        public IEnumerator<Box> GetEnumerator() => Children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Children.GetEnumerator();
    }
}