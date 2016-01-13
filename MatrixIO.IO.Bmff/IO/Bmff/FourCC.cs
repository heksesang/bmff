using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatrixIO.IO.Bmff
{
    public struct FourCC
    {
        private uint _FourCC;

        public FourCC(string fourcc) : this(Encoding.UTF8.GetBytes(fourcc)) { }
        public FourCC(byte[] fourcc)
        {
            _FourCC = BitConverter.ToUInt32(fourcc, 0).NetworkToHostOrder();
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
        public static implicit operator byte[](FourCC fourcc)
        {
            return fourcc.GetBytes();
        }

        public static bool operator ==(FourCC a, FourCC b)
        {
            if (System.Object.ReferenceEquals(a, b)) return true;

            if (((object)a == null) || ((object)b == null)) return false;

            return a._FourCC == b._FourCC;
        }
        public static bool operator !=(FourCC a, FourCC b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj is FourCC)
            {
                if (((FourCC)obj)._FourCC == _FourCC) return true;
                else return false;
            }
            else return false;
        }

        public override int GetHashCode()
        {
            return (int)_FourCC;
        }

        public byte[] GetBytes()
        {
            return BitConverter.GetBytes(_FourCC.HostToNetworkOrder());
        }
        public override string ToString()
        {
            try
            {
                string fourcc = Encoding.UTF8.GetString(GetBytes(), 0, 4);
                bool hasControlChars = false;
                foreach (char c in fourcc) if (char.IsControl(c)) hasControlChars = true;
                if (!hasControlChars) return fourcc;
            }
            catch (ArgumentException) { } // UTF8 conversion failed

            return String.Format("0x{0:x8}", _FourCC);
        }
    }
}
