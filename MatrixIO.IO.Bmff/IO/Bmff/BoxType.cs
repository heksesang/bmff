using System;

namespace MatrixIO.IO.Bmff
{
    public readonly struct BoxType
    {
        private static readonly byte[] BASE_UUID = {
            0x00, 0x00, 0x00, 0x00, // this portion is replaced by the type
            0x00, 0x11, 0x00, 0x10, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71
        };

        private static readonly FourCC UUID_FOURCC = new FourCC(0x75756964);

        // public BoxType(BoxTypes type) : this((uint)type) { }
        public BoxType(string fourcc) :
            this(new FourCC(fourcc))
        { }

        public BoxType(uint fourcc) :
            this(new FourCC(fourcc))
        { }

        public BoxType(FourCC fourcc)
        {
            FourCC = fourcc;

            byte[] uuidBytes = new byte[16];

            Buffer.BlockCopy(FourCC.GetBytes(), 0, uuidBytes, 0, 4);
            Buffer.BlockCopy(BASE_UUID, 4, uuidBytes, 4, 12);

            UserType = new Guid(uuidBytes);
        }

        public BoxType(Guid uuid)
        {
            UserType = uuid;

            bool isBaseType = true;
            byte[] uuidBytes = uuid.ToByteArray();
            for (int i = 4; i < 16; i++) if (uuidBytes[i] != BASE_UUID[i]) isBaseType = false;

            if (isBaseType)
            {
                FourCC = new FourCC(BitConverter.ToUInt32(uuidBytes, 0).NetworkToHostOrder());
            }

            else FourCC = UUID_FOURCC;
        }

        public FourCC FourCC { get; }

        public Guid UserType { get; }

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
            return a.UserType == b.UserType;
        }

        public static bool operator !=(BoxType a, BoxType b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return obj is BoxType other && other.UserType == UserType;
        }

        public override int GetHashCode() => UserType.GetHashCode();

        public override string ToString()
        {
            return FourCC == UUID_FOURCC ? $"uuid({UserType})" : FourCC.ToString();
        }
    }
}