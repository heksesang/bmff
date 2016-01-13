using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MatrixIO.IO.MpegTs.Descriptors;

namespace MatrixIO.IO.MpegTs.Tables
{
    public class DescriptionTable : TsTable<string>
    {
        public IList<TsDescriptor> Descriptors { get; private set; } 
        public DescriptionTable() : base() {}
        public DescriptionTable(byte[] buffer, int offset, int length) : base(buffer, offset, length)
        {
            Descriptors = Portability.CreateList<TsDescriptor>();

            while (_position < _sectionLengthStartOffset + SectionLength - 4 && _position < offset + length - 4)
            {
                var descriptor = new UnknownDescriptor(buffer, _position);
                Descriptors.Add(descriptor);
                _position += descriptor.Length;
            }
        }
    }
}
