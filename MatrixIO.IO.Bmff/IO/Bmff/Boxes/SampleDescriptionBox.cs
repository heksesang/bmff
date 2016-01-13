using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Sample Description Box ("stsd")
    /// </summary>
    [Box("stsd", "Sample Description Box")]
    public class SampleDescriptionBox : FullBox, ISuperBox
    {
        public SampleDescriptionBox() : base() { }
        public SampleDescriptionBox(Stream stream) : base(stream) { }

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
