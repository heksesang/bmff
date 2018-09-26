using System;
using System.Buffers.Binary;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using MatrixIO.IO.Bmff.Boxes;

namespace MatrixIO.IO.Bmff
{
    public class BmffReader : ISuperBox, IEnumerable<Box>
    {
        private readonly Stack<Box> _boxStack = new Stack<Box>();
        private IList<Box> _rootBoxes;

        public BmffReader(Stream stream)
        {
            if (stream.CanSeek && stream.Position != 0)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }

            BaseStream = stream;
        }

        public Stream BaseStream { get; }

        public IList<Box> RootBoxes => _rootBoxes ?? (_rootBoxes = new List<Box>(GetBoxes()));

        public bool RandomAccess => BaseStream.CanSeek;

        public int Depth => _boxStack.Count;

        public Box CurrentBox => _boxStack.Peek();

        public bool HasChildren => CurrentBox is ISuperBox;

        /// <summary>
        /// Traverses the tree of Boxes in file order (depth-first)
        /// </summary>
        /// <returns>IEnumerable collection of Box types in file order.</returns>
        public IEnumerable<Box> GetBoxes()
        {
            Box box = null;

            do
            {
                box = Box.FromStream(BaseStream);

                if (box != null)
                {
                    yield return box;
                }
            }
            while (box != null);
        }

        /// <summary>
        /// Seeks to the beginning of the file and reads the "ftyp" Box.
        /// </summary>
        /// <returns>FileTypeBox</returns>
        [Obsolete]
        public FileTypeBox GetFileTypeBox()
        {
            if (BaseStream.CanSeek && BaseStream.Position != 0)
            {
                BaseStream.Seek(0, SeekOrigin.Begin);
            }

            // TODO: Support files where "ftyp" is not the first FourCC like JPEG2000.

            return Box.FromStream<FileTypeBox>(BaseStream);
        }

        /// <summary>
        /// Seek to the end of the file and returns the "mfro" Box for a 
        /// Microsoft Smooth Streaming PIFF format file that gives
        /// you the chunk offsets within the file.
        /// </summary>
        /// <returns>MovieFragmentRandomAccessBox</returns>
        // TODO: Move to a new MatrixIO.IO.Piff.??? class
        public MovieFragmentRandomAccessBox GetMovieFragmentRandomAccessBox()
        {
            BaseStream.Seek(-24, SeekOrigin.End);

            byte[] mfrobuf = BaseStream.ReadBytes(24);

            uint mfraOffset;

            if (BinaryPrimitives.ReadUInt32BigEndian(mfrobuf.AsSpan(12, 4)) == 0x6d66726f ||
                BinaryPrimitives.ReadUInt32BigEndian(mfrobuf.AsSpan(4, 4)) == 0x6d66726f) // 'mfro'
            {
                mfraOffset = BinaryPrimitives.ReadUInt32BigEndian(mfrobuf.AsSpan(20, 4));
            }
            else
            {
                return null;
            }

            BaseStream.Seek(-mfraOffset, SeekOrigin.End);

            return new MovieFragmentRandomAccessBox(BaseStream);
        }

        IList<Box> ISuperBox.Children => RootBoxes;

        IEnumerator<Box> IEnumerable<Box>.GetEnumerator() => RootBoxes.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => RootBoxes.GetEnumerator();
    }
}