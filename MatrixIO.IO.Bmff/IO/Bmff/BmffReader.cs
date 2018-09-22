using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MatrixIO.IO.Bmff
{
    public class BmffReader : ISuperBox, IEnumerable<Box>
    {
        private readonly Stream _baseStream;
        public Stream BaseStream => _baseStream;

        private IList<Box> _rootBoxes;
        public IList<Box> RootBoxes
        {
            get
            {
#pragma warning disable 612,618
                if (_rootBoxes is null) return _rootBoxes = new List<Box>(GetBoxes());
#pragma warning restore 612,618
                return _rootBoxes;
            }
        }

        public bool RandomAccess => _baseStream.CanSeek;

        #region Navigation

        private readonly Stack<Box> _boxStack = new Stack<Box>();

        public int Depth => _boxStack.Count;

        public Box CurrentBox => _boxStack.Peek();

        public bool HasChildren => CurrentBox is ISuperBox;

        #endregion

        public BmffReader(Stream stream)
        {
            if (stream.CanSeek && stream.Position != 0) stream.Seek(0, SeekOrigin.Begin);
            _baseStream = stream;
        }

        [Obsolete("Use the BaseMedia class instead.")]
        public void Scan()
        {
            GetBoxes();
        }

        /// <summary>
        /// Traverses the tree of Boxes in file order (depth-first)
        /// </summary>
        /// <returns>IEnumerable collection of Box types in file order.</returns>
        [Obsolete("Use the BaseMedia class instead.")]
        public IEnumerable<Box> GetBoxes()
        {
            Box box = null;
            do
            {
                box = Box.FromStream(_baseStream);
                if (box != null) yield return box;
            } while (box != null);
        }

        /// <summary>
        /// Seeks to the beginning of the file and reads the "ftyp" Box.
        /// </summary>
        /// <returns>FileTypeBox</returns>
        [Obsolete]
        public Boxes.FileTypeBox GetFileTypeBox()
        {
            if (_baseStream.CanSeek && _baseStream.Position != 0)
            {
                _baseStream.Seek(0, SeekOrigin.Begin);
            }

            // TODO: Support files where "ftyp" is not the first FourCC like JPEG2000.
            return Box.FromStream<Boxes.FileTypeBox>(_baseStream);
        }

        /// <summary>
        /// Seek to the end of the file and returns the "mfro" Box for a Microsoft Smooth Streaming PIFF format file that gives
        /// you the chunk offsets within the file.
        /// </summary>
        /// <returns>MovieFragmentRandomAccessBox</returns>
        // TODO: Move to a new MatrixIO.IO.Piff.??? class
        public Boxes.MovieFragmentRandomAccessBox GetMovieFragmentRandomAccessBox()
        {
            _baseStream.Seek(-24, SeekOrigin.End);

            byte[] mfrobuf = _baseStream.ReadBytes(24);

            uint mfraOffset;
            if (BitConverter.ToUInt32(mfrobuf, 12).NetworkToHostOrder() == 0x6d66726f ||
                BitConverter.ToUInt32(mfrobuf, 4).NetworkToHostOrder() == 0x6d66726f) // 'mfro'
            {
                mfraOffset = BitConverter.ToUInt32(mfrobuf, 20).NetworkToHostOrder();
            }
            else
            {
                return null;
            }

            _baseStream.Seek(-mfraOffset, SeekOrigin.End);

            return new Boxes.MovieFragmentRandomAccessBox(_baseStream);
        }

        [Obsolete("Use the BaseMedia class instead.")]
        IList<Box> ISuperBox.Children => RootBoxes;

        [Obsolete("Use the BaseMedia class instead.")]
        IEnumerator<Box> IEnumerable<Box>.GetEnumerator() => RootBoxes.GetEnumerator();

        [Obsolete("Use the BaseMedia class instead.")]
        IEnumerator IEnumerable.GetEnumerator() => RootBoxes.GetEnumerator();
    }
}