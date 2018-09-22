using System;

namespace MatrixIO.IO
{
    public readonly struct ByteRange : IEquatable<ByteRange>
    {
        public ByteRange(long start, long end)
        {
            if (start < 0 || end < 0) throw new FormatException("ByteRange does not support negative Start and End values.");

            if (start > end)
            {
                Start = end;
                End = start;
            }
            else
            {
                Start = start;
                End = end;
            }
        }

        public long Start { get; }

        public long End { get; }

        public long Length => End - Start;

        public bool Contains(long offset)
        {
            if (offset >= Start && offset <= End) return true;
            else return false;
        }

        public bool Contains(ByteRange range)
        {
            if (range.Start >= Start && range.End <= End) return true;
            return false;
        }

        public bool Contains(long start, long end)
        {
            if (start >= Start && end <= End) return true;

            else return false;
        }

        public bool Overlaps(ByteRange range)
        {
            return Contains(range.Start) || Contains(range.End);
        }

        public bool Overlaps(long start, long end)
        {
            return Contains(start) || Contains(end);
        }

        #region Operator Overloads
        public static bool operator <(ByteRange a, ByteRange b)
        {
            if (a.Start > b.Start && a.End <= b.End) return true;
            else if (a.Start >= b.Start && a.End < b.End) return true;
            else return false;
        }
        public static bool operator >(ByteRange a, ByteRange b)
        {
            if (a.Start < b.Start && a.End >= b.End) return true;
            else if (a.Start <= b.Start && a.End > b.End) return true;
            else return false;
        }
        public static bool operator ==(ByteRange a, ByteRange b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(ByteRange a, ByteRange b)
        {
            return !(a == b);
        }
        public static bool operator >=(ByteRange a, ByteRange b)
        {
            if (a == b) return true;
            else if (a > b) return true;
            else return false;
        }
        public static bool operator <=(ByteRange a, ByteRange b)
        {
            if (a == b) return true;
            else if (a < b) return true;
            else return false;
        }

        public bool Equals(ByteRange other)
        {
            return Start == other.Start && End == other.End;
        }

        public override bool Equals(object obj)
        {
            return obj is ByteRange other && Equals(other);
        }

        private const long TOP_MASK = unchecked((long)0xFFFFFFFF00000000);
        private const long BOTTOM_MASK = 0x00000000FFFFFFFF;

        public override int GetHashCode()
        {
            int hashcode = (int)((Start & TOP_MASK) >> 32);
            hashcode ^= (int)(Start & BOTTOM_MASK);
            hashcode ^= (int)((End & TOP_MASK) >> 32);
            hashcode ^= (int)(End & BOTTOM_MASK);

            return hashcode;
        }
        #endregion

        public static readonly ByteRange MaxValue = new ByteRange(0, long.MaxValue);
        public static readonly ByteRange MinValue = new ByteRange(0, 0);
    }
}
