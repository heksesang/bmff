using System;
using System.IO;
using MatrixIO.IO;

namespace MatrixIO.IO.Ebml
{
    public struct ClassIdentifier : IEquatable<ClassIdentifier>, IComparable<ClassIdentifier>
    {
        private readonly uint _classIdentifier;

        public ClassIdentifier(int value) : this(unchecked((uint)value)) { }
        public ClassIdentifier(uint value)
        {
            _classIdentifier = value;
        }

        public ClassIdentifier(byte a)
        {
            _classIdentifier = (uint)a & 0x1F;
        }
        public ClassIdentifier(byte a, byte b)
        {
            _classIdentifier = (uint) (a << 8 | b) & 0x3FFF;
        }
        public ClassIdentifier(byte a, byte b, byte c)
        {
            _classIdentifier = (uint)(a << 16 | b << 8 | c) & 0x3FFF;
        }
        public ClassIdentifier(byte a, byte b, byte c, byte d)
        {
            _classIdentifier = (uint)(a << 24 | b << 16 | c << 8 | d) & 0x3FFF;
        }
        public ClassIdentifier(byte[] bytes, int offset = 0)
        {
            byte firstByte = bytes[offset];
            if (firstByte > 0x7F)
                _classIdentifier = bytes[offset];
            else if (firstByte > 0x3F)
                _classIdentifier = (uint) bytes[offset] << 8 | bytes[offset + 1];
            else if (firstByte > 0x1F)
                _classIdentifier = (uint)bytes[offset] << 16 | (uint)bytes[offset + 1] << 8 | bytes[offset + 2];
            else if (firstByte > 0x0F)
                _classIdentifier = (uint)bytes[offset] << 24 | (uint)bytes[offset + 1] << 16 |
                                   (uint)bytes[offset + 2] << 8 | bytes[offset + 3];
            else throw new ArgumentException("Invalid length specifier in Class Identifier.");
        }
        public ClassIdentifier(Stream source)
        {
            var bytes = new byte[4];
            byte firstByte = source.ReadOneByte();
            
            if(firstByte > 0x7F)
            {
                _classIdentifier = firstByte;
            }
            else if(firstByte > 0x3F)
            {
                bytes[0] = firstByte;
                source.Read(bytes, 1, 1);
                _classIdentifier = (uint)bytes[0] << 8 | bytes[1];
            }
            else if (firstByte > 0x1F)
            {
                bytes[0] = firstByte;
                source.Read(bytes, 1, 2);
                _classIdentifier = (uint) bytes[0] << 16 | (uint) bytes[1] << 8 | bytes[2];
            }
            else if (firstByte > 0x0F)
            {
                bytes[0] = firstByte;
                source.Read(bytes, 1, 3);
                _classIdentifier = (uint) bytes[0] << 24 | (uint) bytes[1] << 16 |
                                   (uint) bytes[2] << 8 | bytes[3];
            }
            else throw new ArgumentException("Invalid length specifier in Class Identifier.");

        }

        public byte[] GetBytes()
        {
            byte[] bytes = null;
            if (_classIdentifier > 0xFF)
            {
                bytes = new byte[1];
                bytes[0] = (byte) _classIdentifier;
            }
            else if (_classIdentifier > 0xFFFF)
            {
                bytes = new byte[2];
                bytes[0] = (byte) (_classIdentifier >> 8);
                bytes[1] = (byte) (_classIdentifier & 0xFF);
            }
            else if (_classIdentifier > 0xFFFFFF)
            {
                bytes = new byte[3];
                bytes[0] = (byte) (_classIdentifier >> 16);
                bytes[1] = (byte) (_classIdentifier >> 8 & 0xFF);
                bytes[2] = (byte) (_classIdentifier & 0xFF);
            }
            else if (_classIdentifier > 0xFFFFFFFF)
            {
                bytes = new byte[4];
                bytes[0] = (byte)(_classIdentifier >> 24);
                bytes[1] = (byte)(_classIdentifier >> 16 & 0xFF);
                bytes[1] = (byte)(_classIdentifier >> 8 & 0xFF);
                bytes[2] = (byte)(_classIdentifier & 0xFF);
            }
            return bytes;
        }

        public bool Equals(ClassIdentifier other)
        {
            return _classIdentifier.Equals(other._classIdentifier);
        }

        public override int GetHashCode()
        {
            return _classIdentifier.GetHashCode();
        }

        public int CompareTo(ClassIdentifier other)
        {
            return _classIdentifier.CompareTo(other._classIdentifier);
        }

        public static implicit operator uint(ClassIdentifier classIdentifier)
        {
            return classIdentifier._classIdentifier;
        }
        public static explicit operator ClassIdentifier(uint value)
        {
            return new ClassIdentifier(value);
        }
    }
}
