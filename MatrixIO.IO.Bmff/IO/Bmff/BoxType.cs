using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatrixIO.IO.Bmff
{
    public struct BoxType
    {
        private static readonly byte[] BASE_UUID = {
            0x00, 0x00, 0x00, 0x00, // this portion is replaced by the type
            0x00, 0x11, 0x00, 0x10, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71 
        };

        private static readonly FourCC UUID_FOURCC = new FourCC(0x75756964);

        private FourCC _FourCC;
        public FourCC FourCC { get { return _FourCC; } }

        private Guid _UserType;
        public Guid UserType { get { return _UserType; } }

        //public BoxType(BoxTypes type) : this((uint)type) { }
        public BoxType(string fourcc) : this(new FourCC(fourcc)) { }
        public BoxType(uint fourcc) : this(new FourCC(fourcc)) {}
        public BoxType(FourCC fourcc)
        {
            _FourCC = fourcc;

            byte[] uuidBytes = new byte[16];
            Buffer.BlockCopy(_FourCC.GetBytes(), 0, uuidBytes, 0, 4);
            Buffer.BlockCopy(BASE_UUID, 4, uuidBytes, 4, 12);
            
            _UserType = new Guid(uuidBytes);
        }
        public BoxType(Guid uuid)
        {
            _UserType = uuid;

            bool isBaseType = true;
            byte[] uuidBytes = uuid.ToByteArray();
            for (int i = 4; i < 16; i++) if (uuidBytes[i] != BASE_UUID[i]) isBaseType=false;

            if (isBaseType) _FourCC = new FourCC(BitConverter.ToUInt32(uuidBytes, 0).NetworkToHostOrder());
            else _FourCC = UUID_FOURCC;
        }

        public static implicit operator BoxType(FourCC fourcc)
        {
            return new BoxType(fourcc);
        }
        public static implicit operator BoxType(string fourcc)
        {
            return new BoxType(fourcc);
        }
        public static implicit operator BoxType(uint fourcc)
        {
            return new BoxType(fourcc);
        }

        public static bool operator ==(BoxType a, BoxType b)
        {
            if (System.Object.ReferenceEquals(a, b)) return true;

            if (((object)a == null) || ((object)b == null)) return false;

            return a._UserType == b._UserType;
        }
        public static bool operator !=(BoxType a, BoxType b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj) && ((BoxType)obj)._UserType == _UserType;
        }

        public override int GetHashCode()
        {
            return _UserType.GetHashCode();
        }

        public override string ToString()
        {
            return FourCC == UUID_FOURCC ? string.Format("uuid({0})", UserType.ToString()) : FourCC.ToString();
        }
    }
}
