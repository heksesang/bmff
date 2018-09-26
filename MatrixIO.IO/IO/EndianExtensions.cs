using System;
using System.Buffers.Binary;

namespace MatrixIO.IO
{
    public static class EndianExtensions
    {
        public static short NetworkToHostOrder(this short value)
        {
            return BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value;
        }

        public static int NetworkToHostOrder(this int value)
        {
            return BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value;
        }

        public static long NetworkToHostOrder(this long value)
        {
            return BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value;
        }

        public static ushort NetworkToHostOrder(this ushort value)
        {
            return BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value;
        }

        public static uint NetworkToHostOrder(this uint value)
        {
            return BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value;
        }

        public static ulong NetworkToHostOrder(this ulong value)
        {
            return BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value;
        }

        public static short HostToNetworkOrder(this short value)
        {
            return BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value;
        }

        public static int HostToNetworkOrder(this int value)
        {
            return BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value;
        }

        public static long HostToNetworkOrder(this long value)
        {
            return BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value;
        }

        public static ushort HostToNetworkOrder(this ushort value)
        {
            return BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value;
        }

        public static uint HostToNetworkOrder(this uint value)
        {
            return BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value;
        }

        public static ulong HostToNetworkOrder(this ulong value)
        {
            return BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value;
        }
    }
}