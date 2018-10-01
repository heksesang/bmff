using System;
using System.Buffers.Binary;
using System.Text;

namespace MatrixIO.IO.Bmff
{
    public readonly struct FourCC
    {
        private readonly uint _FourCC;

        public FourCC(string fourcc)
            : this(Encoding.UTF8.GetBytes(fourcc)) { }

        public FourCC(ReadOnlySpan<byte> fourcc)
        {
            _FourCC = BinaryPrimitives.ReadUInt32BigEndian(fourcc);
        }

        public FourCC(int fourcc) : this((uint)fourcc) { }

        [CLSCompliant(false)]
        public FourCC(uint fourcc)
        {
            _FourCC = fourcc;
        }

        public static implicit operator int(FourCC fourcc)
        {
            return (int)fourcc._FourCC;
        }

        public static implicit operator FourCC(int fourcc)
        {
            return new FourCC(fourcc);
        }

        public static implicit operator uint(FourCC fourcc)
        {
            return fourcc._FourCC;
        }

        public static implicit operator FourCC(uint fourcc)
        {
            return new FourCC(fourcc);
        }

        public static implicit operator string(FourCC fourcc)
        {
            return fourcc.ToString();
        }

        public static implicit operator FourCC(string fourcc)
        {
            return new FourCC(fourcc);
        }

        public static implicit operator byte[] (FourCC fourcc)
        {
            return fourcc.GetBytes();
        }

        public static bool operator ==(FourCC a, FourCC b)
        {
            return a._FourCC == b._FourCC;
        }

        public static bool operator !=(FourCC a, FourCC b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return obj is FourCC other && this._FourCC == other._FourCC;
        }

        public override int GetHashCode() => (int)_FourCC;

        public byte[] GetBytes()
        {
            byte[] buffer = new byte[4];

            BinaryPrimitives.WriteUInt32BigEndian(buffer, _FourCC);

            return buffer;
        }

        public override string ToString()
        {
            try
            {
                string fourcc = Encoding.ASCII.GetString(GetBytes(), 0, 4);

                bool hasControlChars = false;

                foreach (char c in fourcc)
                {
                    if (char.IsControl(c))
                    {
                        hasControlChars = true;
                    }
                }

                if (!hasControlChars) return fourcc;
            }
            catch (ArgumentException) { } // UTF8 conversion failed

            return $"0x{_FourCC:x8}";
        }
    }
}