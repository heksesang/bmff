using System;

namespace MatrixIO.IO
{
    public static class EndianExtensions
    {
        #region ByteSwap
        public static ushort ByteSwap(this ushort v)
        {
            return (ushort)(((v & 0xff) << 8) | ((v >> 8) & 0xff));
        }
        public static uint ByteSwap(this uint v)
        {
            return (uint)(((((ushort)v).ByteSwap() & 0xffff) << 0x10) |
                           (((ushort)(v >> 0x10)).ByteSwap() & 0xffff));
        }
        public static ulong ByteSwap(this ulong v)
        {
            return (ulong)(((((uint)v).ByteSwap() & 0xffffffffL) << 0x20) |
                            (((uint)(v >> 0x20)).ByteSwap() & 0xffffffffL));
        }
        public static short ByteSwap(this short v)
        {
            return (short)(((v & 0xff) << 8) | ((v >> 8) & 0xff));
        }
        public static int ByteSwap(this int v)
        {
            return (int)(((((short)v).ByteSwap() & 0xffff) << 0x10) |
                          (((short)(v >> 0x10)).ByteSwap() & 0xffff));
        }
        public static long ByteSwap(this long v)
        {
            return (long)(((((int)v).ByteSwap() & 0xffffffffL) << 0x20) |
                           (((int)(v >> 0x20)).ByteSwap() & 0xffffffffL));
        }
        #endregion

        #region NetworkToHostOrder
        public static short NetworkToHostOrder(this short value)
        {
            if (BitConverter.IsLittleEndian) return value.ByteSwap();
            return value;
        }
        public static int NetworkToHostOrder(this int value)
        {
            if (BitConverter.IsLittleEndian) return value.ByteSwap();
            return value;
        }
        public static long NetworkToHostOrder(this long value)
        {
            if (BitConverter.IsLittleEndian) return value.ByteSwap();
            return value;
        }
        public static ushort NetworkToHostOrder(this ushort value)
        {
            if (BitConverter.IsLittleEndian) return value.ByteSwap();
            return value;
        }
        public static uint NetworkToHostOrder(this uint value)
        {
            if (BitConverter.IsLittleEndian) return value.ByteSwap();
            return value;
        }
        public static ulong NetworkToHostOrder(this ulong value)
        {
            if (BitConverter.IsLittleEndian) return value.ByteSwap();
            return value;
        }
        #endregion NetworkToHostOrder

        #region HostToNetworkOrder
        public static short HostToNetworkOrder(this short value)
        {
            if (BitConverter.IsLittleEndian) return value.ByteSwap();
            return value;
        }
        public static int HostToNetworkOrder(this int value)
        {
            if (BitConverter.IsLittleEndian) return value.ByteSwap();
            return value;
        }
        public static long HostToNetworkOrder(this long value)
        {
            if (BitConverter.IsLittleEndian) return value.ByteSwap();
            return value;
        }
        public static ushort HostToNetworkOrder(this ushort value)
        {
            if (BitConverter.IsLittleEndian) return value.ByteSwap();
            return value;
        }
        public static uint HostToNetworkOrder(this uint value)
        {
            if (BitConverter.IsLittleEndian) return value.ByteSwap();
            return value;
        }
        public static ulong HostToNetworkOrder(this ulong value)
        {
            if (BitConverter.IsLittleEndian) return value.ByteSwap();
            return value;
        }
        #endregion HostToNetworkOrder
    }
}
