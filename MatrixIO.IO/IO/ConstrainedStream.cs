using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace MatrixIO.IO
{
    public class ConstrainedStream : Stream
    {
        private Stream _BaseStream;

        private Stack<ByteRange> _ConstraintStack = new Stack<ByteRange>();

        private ByteRange _CurrentConstraint = ByteRange.MaxValue;
        public ByteRange CurrentConstraint
        {
            get
            {
                return _CurrentConstraint;
            }
        }

        public ConstrainedStream(Stream baseStream)
        {
            _BaseStream = baseStream;
        }

        public static ConstrainedStream WrapStream(Stream baseStream)
        {
            if (baseStream is ConstrainedStream) return (ConstrainedStream)baseStream;
            else return new ConstrainedStream(baseStream);
        }
        public static Stream UnwrapStream(Stream stream)
        {
            if (stream is ConstrainedStream) return ((ConstrainedStream)stream)._BaseStream;
            else return stream;
        }

        public void PushConstraint(long length)
        {
            PushConstraint(Position, length);
        }
        public void PushConstraint(long offset, long length)
        {

            if (offset < _CurrentConstraint.Start)
            {
                Trace.WriteLine(String.Format("Attempted to set a new constraint ({0},{1}) offset outside the bounds of the existing constraint({2},{3}).", offset, length, _CurrentConstraint.Start, _CurrentConstraint.Length), "WARNING");
                offset = _CurrentConstraint.Start;
            }
            if (offset + length > _CurrentConstraint.End)
            {
                Trace.WriteLine(String.Format("Attempted to set a new constraint ({0},{1}) length outside the bounds of the existing constraint({2},{3}).", offset, length, _CurrentConstraint.Start, _CurrentConstraint.Length), "WARNING");
                length = _CurrentConstraint.End - offset;
            }

            if (_CurrentConstraint != null) _ConstraintStack.Push(_CurrentConstraint);
            _CurrentConstraint = new ByteRange(offset, offset + length);
        }

        public void PopConstraint()
        {
            if (_ConstraintStack.Count > 0) _CurrentConstraint = _ConstraintStack.Pop();
            else
            {
                Trace.WriteLine("Attempted to pop constraint too many times.", "WARNING");
                _CurrentConstraint = ByteRange.MaxValue;
            }
        }

        public override bool CanRead
        {
            get { return _BaseStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return _BaseStream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return _BaseStream.CanWrite; }
        }

        public override void Flush()
        {
            _BaseStream.Flush();
        }

        public override long Length
        {
            get 
            { 
                return Math.Min(_BaseStream.Length, _CurrentConstraint.End); 
            }
        }

        public long _Count;
        public override long Position
        {
            get
            {
                if (CanSeek) return _BaseStream.Position;
                else return _Count;
            }
            set
            {
                _BaseStream.Position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            long position = Position;
            if (position + count > _CurrentConstraint.End)
            {
                int availableCount = checked((int)(_CurrentConstraint.End - position));
                Trace.WriteLine(String.Format("Attempt to read {0} bytes past end of constrained region.", count - availableCount), "WARNING");
                if (availableCount > 0)
                {
                    Trace.WriteLine("Returning partial result.", "WARNING");
                    int actualCount = _BaseStream.Read(buffer, offset, availableCount);
                    if (!CanSeek) _Count += actualCount;
                    return actualCount;
                }
                else return 0;
            }
            else
            {
                int actualCount = _BaseStream.Read(buffer, offset, count);
                if (!CanSeek) _Count += actualCount;
                return actualCount;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
           if ((origin == SeekOrigin.Begin && !_CurrentConstraint.Contains(offset)) ||
               (origin == SeekOrigin.Current && !_CurrentConstraint.Contains(Position + offset)) ||
               (origin == SeekOrigin.End && !_CurrentConstraint.Contains(Length - 1 - Math.Abs(offset)))) 
               throw new EndOfStreamException("Attempt to seek beyond constrained region.");

            return _BaseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _BaseStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (Position + count > _CurrentConstraint.End) throw new IOException("Attempt to write past end of constrained region.");
            if (!CanSeek) _Count += count;
            _BaseStream.Write(buffer, offset, count);
        }
    }
}
