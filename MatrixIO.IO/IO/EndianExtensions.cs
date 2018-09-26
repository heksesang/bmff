using System;
using System.Buffers.Binary;

namespace MatrixIO.IO
{
    internal static class EndianExtensions
    {
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