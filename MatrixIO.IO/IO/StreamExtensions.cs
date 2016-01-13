using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace MatrixIO.IO
{
    public static class StreamExtensions
    {
        public static byte ReadOneByte(this Stream stream)
        {
            return ReadBytes(stream, 1)[0];
        }

        public static byte[] ReadBytes(this Stream stream, int count, bool partial = true)
        {
            byte[] buffer = new byte[count];
            int len = stream.Read(buffer, 0, count);
            if (len < count)
            {
                if (!partial || len == 0) throw new EndOfStreamException();
                Array.Resize(ref buffer, len);
            }
            return buffer;
        }

        public static void WriteOneByte(this Stream stream, byte b)
        {
            byte[] buffer = new byte[] { b };
            stream.Write(buffer, 0, 1);
        }
        public static void WriteBytes(this Stream stream, byte[] bytes)
        {
            stream.Write(bytes, 0, bytes.Length);
        }

        #region BigEndian Read
        public static short ReadBEInt16(this Stream stream)
        {
            return BitConverter.ToInt16(stream.ReadBytes(2, false), 0).NetworkToHostOrder();
        }
        public static ushort ReadBEUInt16(this Stream stream)
        {
            return (ushort)ReadBEInt16(stream);
        }

        public static int ReadBEInt24(this Stream stream)
        {
            byte[] buffer = new byte[4];
            int len = stream.Read(buffer, 0, 3);
            if (len < 3) throw new EndOfStreamException();
            return BitConverter.ToInt32(buffer, 0).NetworkToHostOrder();
        }
        
        public static uint ReadBEUInt24(this Stream stream)
        {
            byte[] buffer = new byte[4];
            stream.Read(buffer, 1, 3);
            return BitConverter.ToUInt32(buffer, 0).NetworkToHostOrder();
        }
        
        public static int ReadBEInt32(this Stream stream)
        {
            return BitConverter.ToInt32(stream.ReadBytes(4, false), 0).NetworkToHostOrder();
        }
        public static uint ReadBEUInt32(this Stream stream)
        {
            return (uint)ReadBEInt32(stream);
        }

        public static long ReadBEInt64(this Stream stream)
        {
            return BitConverter.ToInt64(stream.ReadBytes(8, false), 0).NetworkToHostOrder();
        }
        public static ulong ReadBEUInt64(this Stream stream)
        {
            return (ulong)ReadBEInt64(stream);
        }
        #endregion

        #region BigEndian Write
        public static void WriteBEInt16(this Stream stream, short value)
        {
            byte[] buffer = BitConverter.GetBytes(value.HostToNetworkOrder());
            stream.Write(buffer, 0, buffer.Length);
        }
        public static void WriteBEUInt16(this Stream stream, ushort value)
        {
            byte[] buffer = BitConverter.GetBytes(value.HostToNetworkOrder());
            stream.Write(buffer, 0, buffer.Length);
        }

        public static void WriteBEInt24(this Stream stream, int value)
        {
            WriteBEUInt24(stream, (uint)value);
        }
        public static void WriteBEUInt24(this Stream stream, uint value)
        {
            if (value > 16777215) throw new OverflowException();
            byte[] buffer = BitConverter.GetBytes(value.HostToNetworkOrder());
            stream.Write(buffer, 1, buffer.Length - 1);
        }

        public static void WriteBEInt32(this Stream stream, int value)
        {
            byte[] buffer = BitConverter.GetBytes(value.HostToNetworkOrder());
            stream.Write(buffer, 0, buffer.Length);
        }
        public static void WriteBEUInt32(this Stream stream, uint value)
        {
            byte[] buffer = BitConverter.GetBytes(value.HostToNetworkOrder());
            stream.Write(buffer, 0, buffer.Length);
        }

        public static void WriteBEInt64(this Stream stream, long value)
        {
            byte[] buffer = BitConverter.GetBytes(value.HostToNetworkOrder());
            stream.Write(buffer, 0, buffer.Length);
        }
        public static void WriteBEUInt64(this Stream stream, ulong value)
        {
            byte[] buffer = BitConverter.GetBytes(value.HostToNetworkOrder());
            stream.Write(buffer, 0, buffer.Length);
        }
        #endregion

        public static string ReadNullTerminatedUTF8String(this Stream stream)
        {
            List<byte> bytes = new List<byte>();
            byte b;
            try
            {
                while ((b = stream.ReadOneByte()) > 0)
                {
                    bytes.Add(b);
                    if ((b & 0x80) == 0x80) // 0x1xxxxxxx
                    {
                        int continuations = 0;
                        if (((b >> 5) & 0x06) == 0x06) continuations = 1;      // 0x110xxxxx
                        else if (((b >> 4) & 0x0E) == 0x0E) continuations = 2; // 0x1110xxxx
                        else if (((b >> 3) & 0x0E) == 0x0E) continuations = 3; // 0x11110xxx

                        for (int i = 0; i < continuations; i++)
                        {
                            byte c = stream.ReadOneByte();
                            if (((c >> 6) & 0x02) == 0x02) bytes.Add(c); // 0x10xxxxxx
                            else throw new FormatException("Invalid UTF-8 while reading null-terminated UTF-8 string.");
                        }
                    }
                }
            }
            catch (EndOfStreamException) 
            {
                Trace.WriteLine("Last warning was expected.", "INFO");
            }

            if (bytes.Count > 0) return Encoding.UTF8.GetString(bytes.ToArray(), 0, bytes.Count);
            else return null;
        }

        public static int WriteNullTerminatedUTF8String(this Stream stream, string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                stream.WriteOneByte(0);
                return 1;
            }
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            stream.Write(bytes, 0, bytes.Length);
            stream.WriteOneByte(0);
            return bytes.Length + 1;
        }

        public static int WriteUTF8String(this Stream stream, string text)
        {
            if (String.IsNullOrEmpty(text)) return 0;
            
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            stream.Write(bytes, 0, bytes.Length);
            return bytes.Length;
        }
    }
}
