using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace MatrixIO.IO
{
    public sealed class ConstrainedStream : Stream
    {
        private Stream _baseStream;
        private ByteRange _currentConstraint = ByteRange.MaxValue;

        private Stack<ByteRange> _ConstraintStack = new Stack<ByteRange>();

        public ByteRange CurrentConstraint => _currentConstraint;

        public ConstrainedStream(Stream baseStream)
        {
            _baseStream = baseStream;
        }

        public static ConstrainedStream WrapStream(Stream baseStream)
        {
            return baseStream is ConstrainedStream constrainedStream
                ? constrainedStream
                : new ConstrainedStream(baseStream);
        }

        public static Stream UnwrapStream(Stream stream)
        {
            return stream is ConstrainedStream constrainedStream
                ? constrainedStream._baseStream
                : stream;
        }

        public void PushConstraint(long length)
        {
            PushConstraint(Position, length);
        }

        public void PushConstraint(long offset, long length)
        {
            if (offset < _currentConstraint.Start)
            {
                Trace.WriteLine($"Attempted to set a new constraint ({offset},{length}) offset outside the bounds of the existing constraint({_currentConstraint.Start},{_currentConstraint.Length}).", "WARNING");
                offset = _currentConstraint.Start;
            }

            if (offset + length > _currentConstraint.End)
            {
                Trace.WriteLine($"Attempted to set a new constraint ({offset},{length}) length outside the bounds of the existing constraint({_currentConstraint.Start},{_currentConstraint.Length}).", "WARNING");
                length = _currentConstraint.End - offset;
            }

            if (_currentConstraint != null)
            {
                _ConstraintStack.Push(_currentConstraint);
            }

            _currentConstraint = new ByteRange(offset, offset + length);
        }

        public void PopConstraint()
        {
            if (_ConstraintStack.Count > 0) _currentConstraint = _ConstraintStack.Pop();
            else
            {
                Trace.WriteLine("Attempted to pop constraint too many times.", "WARNING");
                _currentConstraint = ByteRange.MaxValue;
            }
        }

        public override bool CanRead => _baseStream.CanRead;

        public override bool CanSeek => _baseStream.CanSeek;

        public override bool CanWrite => _baseStream.CanWrite;

        public override void Flush()
        {
            _baseStream.Flush();
        }

        public override long Length => Math.Min(_baseStream.Length, _currentConstraint.End);

        public long _count;
        public override long Position
        {
            get => CanSeek ? _baseStream.Position : _count;
            set => _baseStream.Position = value;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            long position = Position;
            if (position + count > _currentConstraint.End)
            {
                int availableCount = checked((int)(_currentConstraint.End - position));
                Trace.WriteLine(string.Format("Attempt to read {0} bytes past end of constrained region.", count - availableCount), "WARNING");
                if (availableCount > 0)
                {
                    Trace.WriteLine("Returning partial result.", "WARNING");
                    int actualCount = _baseStream.Read(buffer, offset, availableCount);
                    if (!CanSeek) _count += actualCount;
                    return actualCount;
                }
                else return 0;
            }
            else
            {
                int actualCount = _baseStream.Read(buffer, offset, count);
                if (!CanSeek) _count += actualCount;
                return actualCount;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if ((origin == SeekOrigin.Begin && !_currentConstraint.Contains(offset)) ||
                (origin == SeekOrigin.Current && !_currentConstraint.Contains(Position + offset)) ||
                (origin == SeekOrigin.End && !_currentConstraint.Contains(Length - 1 - Math.Abs(offset))))
            {
                throw new EndOfStreamException("Attempt to seek beyond constrained region.");
            }

            return _baseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _baseStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (Position + count > _currentConstraint.End)
            {
                throw new IOException("Attempt to write past end of constrained region.");
            }

            if (!CanSeek)
            {
                _count += count;
            }

            _baseStream.Write(buffer, offset, count);
        }
    }
}
