using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Movie Data Box ("mdat")
    /// </summary>
    [Box("mdat", "Movie Data Box")]
    public class MovieDataBox : Box, IContentBox
    {
        public MovieDataBox() : base() { }
        /// <summary>
        /// Reads an 'mdat' box from a given stream
        /// </summary>
        /// <param name="stream"></param>
        public MovieDataBox(Stream stream) : base(stream) { }

        /// <summary>
        /// Constructs a new box with content from the given stream
        /// </summary>
        /// <param name="stream">Stream containing box content.</param>
        /// <param name="contentOffset">Offset of box content.</param>
        /// <param name="contentSize">Length of box content.</param>
        public MovieDataBox(Stream contentStream, ulong contentOffset, ulong? contentSize)
        {
            if (!contentStream.CanSeek) throw new NotSupportedException("The underlying Box.ToStream() does not presently support reading source content from an unseekable stream.");

            if (contentSize == null)
            {
                throw new NotSupportedException("The underlying Box.ToStream() does not presently support unknown length content.");
                //this.Size = 0;
            } 
            else if (contentSize + 8 > uint.MaxValue)
            {
                this.Size = 1;
                this.LargeSize = (contentSize + 8);
            }
            else
            {
                this.Size = (uint)(contentSize + 16);
            }

            this.ContentOffset = contentOffset;
            this._SourceStream = contentStream;
        }
    }
}
