using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Unknown Box Type
    /// </summary>
    public class UnknownBox : Box, IContentBox
    {
        public UnknownBox() : base() { }
        public UnknownBox(BoxType type) : base() 
        {
            Type = type;
        }
        public UnknownBox(Stream stream) : base(stream) { }
    }
}
