using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MatrixIO.IO.MpegTs
{

    public enum StreamIdentifier : byte
    {
        ProgramStreamMap = 0xBC,
        PrivateStream1 = 0xBD,
        PaddingStream = 0xBE,
        PrivateStream2 = 0xBF,
        #region Numbered Audio Streams
        AudioStream0 = 0xC0,
        AudioStream1 = 0xC1,
        AudioStream2 = 0xC2,
        AudioStream3 = 0xC3,
        AudioStream4 = 0xC4,
        AudioStream5 = 0xC5,
        AudioStream6 = 0xC6,
        AudioStream7 = 0xC7,
        AudioStream8 = 0xC8,
        AudioStream9 = 0xC9,
        AudioStream10 = 0xCA,
        AudioStream11 = 0xCB,
        AudioStream12 = 0xCC,
        AudioStream13 = 0xCD,
        AudioStream14 = 0xCE,
        AudioStream15 = 0xCF,
        AudioStream16 = 0xD0,
        AudioStream17 = 0xD1,
        AudioStream18 = 0xD2,
        AudioStream19 = 0xD3,
        AudioStream20 = 0xD4,
        AudioStream21 = 0xD5,
        AudioStream22 = 0xD6,
        AudioStream23 = 0xD7,
        AudioStream24 = 0xD8,
        AudioStream25 = 0xD9,
        AudioStream26 = 0xDA,
        AudioStream27 = 0xDB,
        AudioStream28 = 0xDC,
        AudioStream29 = 0xDD,
        AudioStream30 = 0xDE,
        AudioStream31 = 0xDF,
        #endregion
        #region Numbered Video Streams
        VideoStream0 = 0xE0,
        VideoStream1 = 0xE1,
        VideoStream2 = 0xE2,
        VideoStream3 = 0xE3,
        VideoStream4 = 0xE4,
        VideoStream5 = 0xE5,
        VideoStream6 = 0xE6,
        VideoStream7 = 0xE7,
        VideoStream8 = 0xE8,
        VideoStream9 = 0xE9,
        VideoStream10 = 0xEA,
        VideoStream11 = 0xEB,
        VideoStream12 = 0xEC,
        VideoStream13 = 0xED,
        VideoStream14 = 0xEE,
        VideoStream15 = 0xEF,
        #endregion
        EcmStream = 0xF0,
        EmmStream = 0xF1,
        DsmCcStream = 0xF2,
        Iso13522Stream = 0xF3,
        TypeAStream = 0xF4,
        TypeBStream = 0xF5,
        TypeCStream = 0xF6,
        TypeDStream = 0xF7,
        TypeEStream = 0xF8,
        AncillaryStream = 0xF9,
        SlPacketizedStream = 0xFA,
        FlexMuxStream = 0xFB,
        // 0xFC-0xFE RESERVED
        ProgramStreamDirectory = 0xFF,
    }


    public class PesPacket
    {
        public StreamIdentifier StreamIdentifier { get; set; }
        public int PacketLength { get; private set; }
        public PESHeader Header { get; set; }
        public byte[] Stuffing { get; set; }
        public byte[] Data { get; set; }

        private const uint PacketStartCodePrefix = 0x00000100;
        private const uint PacketStartCodeMask   = 0xFFFFFF00;
        private const uint ProgramStreamEndCode  = 0x000001B9;

        public PesPacket(byte[] buffer, int offset = 0)
        {
            int position = offset;
            if (buffer[position++] != 0 || buffer[position++] != 0 || buffer[position++] != 1)
                throw new ArgumentException("PES Packet does not start with 0x000001 Start Code Prefix.");
            StreamIdentifier = (StreamIdentifier) buffer[position++];

            PacketLength = (buffer[position++] << 8) | buffer[position++];
            if (PacketLength == 0) PacketLength = buffer.Length - offset - 6;

            if (StreamIdentifier != StreamIdentifier.ProgramStreamMap
                && StreamIdentifier != StreamIdentifier.PaddingStream
                && StreamIdentifier != StreamIdentifier.PrivateStream2
                && StreamIdentifier != StreamIdentifier.EcmStream
                && StreamIdentifier != StreamIdentifier.EmmStream
                && StreamIdentifier != StreamIdentifier.ProgramStreamDirectory
                && StreamIdentifier != StreamIdentifier.DsmCcStream
                && StreamIdentifier != StreamIdentifier.TypeEStream)
            {
                Header = new PESHeader(buffer, position);
                position += Header.Length;

                Data = new byte[PacketLength - Header.Length];
                // TODO: Something may be squirrely here.  position+PacketLength should never be > buffer.Length
                // Either ffmpeg is producing bad PES packets or we are reassembling units incorrectly.
                // Math.Min() here prevents the exception but doesn't fix the underlying problem.
                if (PacketLength > 0) Buffer.BlockCopy(buffer, position, Data, 0, Math.Min(Data.Length, buffer.Length - position));
               
            }
            else if (StreamIdentifier == StreamIdentifier.ProgramStreamMap
                     || StreamIdentifier == StreamIdentifier.PrivateStream2
                     || StreamIdentifier == StreamIdentifier.EcmStream
                     || StreamIdentifier == StreamIdentifier.EmmStream
                     || StreamIdentifier == StreamIdentifier.ProgramStreamDirectory
                     || StreamIdentifier == StreamIdentifier.DsmCcStream
                     || StreamIdentifier == StreamIdentifier.TypeEStream)
            {
                Data = new byte[PacketLength];
                if (PacketLength > 0) Buffer.BlockCopy(buffer, position, Data, 0, Math.Min(Data.Length, buffer.Length - position));
            }
            else if (StreamIdentifier == StreamIdentifier.PaddingStream)
            {
                Stuffing = new byte[PacketLength];
                if (PacketLength > 0) Buffer.BlockCopy(buffer, position, Data, 0, Math.Min(Data.Length, buffer.Length - position));
            }
#if DEBUG
            if(Data!=null && Data.Length > 0)
            {
                Debug.WriteLine("First 40 bytes of PES Data: " + String.Concat(Data.Take(40).Select((b) => b.ToString("X2")).ToArray()));
            }
#endif
        }
    }

    public class PESHeader
    {
        private byte _header;
        private byte _flags;

        public int Length
        {
            get { return 3 + RemainingLength; }
        }

        // 2 bit Marker = 0x02
        // 2 bit ScramblingControl : binary 00 = not scrambled
        public int ScramblingControl
        {
            get { return (_header & 0x30) >> 4; }
            set 
            { 
                if (value > 4) throw new ArgumentException("ScramblingControl must be a value between 0 and 3.");
                _header = (byte) ((_header & 0xCF) | (value << 4));
            }
        }
        // 1 bit Priority
        public bool Priority
        {
            get { return (_header & 0x08) > 0; }
            set { _header = (byte)((_header & 0xF7) | (value ? 0x08 : 0)); }
        }
        // 1 bit DataAlignmentIndicator : 1 indicates packet header is immediately followed by video start code or audio sync word
        public bool DataAlignmentIndicator
        {
            get { return (_header & 0x04) > 0; }
            set { _header = (byte)((_header & 0xFB) | (value ? 0x04 : 0)); }
        }
        // 1 bit IsCopyrighted
        public bool IsCopyrighted
        {
            get { return (_header & 0x02) > 0; }
            set { _header = (byte)((_header & 0xFD) | (value ? 0x02 : 0)); }
        }
        // 1 bit IsOriginal
        public bool IsOriginal
        {
            get { return (_header & 0x01) > 0; }
            set { _header = (byte)((_header & 0xFE) | (value ? 0x01 : 0)); }
        }


        // 1 bit HasPTS
        private bool HasPTS
        {
            get { return (_flags & 0x80) > 0; }
            set { _flags = (byte)((_flags & 0x7F) | (value ? 0x80 : 0)); }
        }
        // 1 bit HasDTS
        private bool HasDTS
        {
            get { return (_flags & 0x40) > 0; }
            set { _flags = (byte)((_flags & 0xBF) | (value ? 0x40 : 0)); }
        }
        // 1 bit ESCRFlag
        private bool HasESCR
        {
            get { return (_flags & 0x20) > 0; }
            set { _flags = (byte)((_flags & 0xDF) | (value ? 0x20 : 0)); }
        }
        // 1 bit ESRateFlag
        private bool HasESRate
        {
            get { return (_flags & 0x10) > 0; }
            set { _flags = (byte)((_flags & 0xEF) | (value ? 0x10 : 0)); }
        }
        // 1 bit DSMTrickModeFlag
        private bool HasDSMTrickMode
        {
            get { return (_flags & 0x08) > 0; }
            set { _flags = (byte)((_flags & 0xF7) | (value ? 0x08 : 0)); }
        }
        // 1 bit AdditionalCopyInfoFlag
        private bool HasAdditionalCopyInfo
        {
            get { return (_flags & 0x04) > 0; }
            set { _flags = (byte)((_flags & 0xFB) | (value ? 0x04 : 0)); }
        }
        // 1 bit CRCFlag
        private bool HasCRC
        {
            get { return (_flags & 0x02) > 0; }
            set { _flags = (byte)((_flags & 0xFD) | (value ? 0x02 : 0)); }
        }
        // 1 bit ExtensionFlag
        private bool HasExtension
        {
            get { return (_flags & 0x01) > 0; }
            set { _flags = (byte)((_flags & 0xFE) | (value ? 0x01 : 0)); }
        }

        // TODO: This should actually be calculated for serialization
        private byte RemainingLength { get; set; }

        private byte[] _pts;
        /// <summary>
        /// Presentation Time Stamp
        /// </summary>
        public TimeSpan? PTS
        {
            get
            {
                if (_pts != null)
                {
                    long pts = ((_pts[0] & 0x0E) << 29) | (_pts[1] << 22) | ((_pts[2] & 0xFE) << 14) | (_pts[3] << 7) |
                               ((_pts[4] & 0xFE) >> 1);
                    return new TimeSpan((pts * 1111111) / 10000); // Convert 90khz to Ticks (multiply by 111.1111)
                }
                return null;
            }
            set
            {
                if (!value.HasValue) _pts = null;
                else
                {
                    var pts = (value.Value.Ticks * 10000) / 1111111;

                }
            }
        }

        private byte[] _dts;
        public TimeSpan? DTS
        {
            get
            {
                if (_dts != null)
                {
                    long dts = ((_dts[0] & 0x0E) << 29) | (_dts[1] << 22) | ((_dts[2] & 0xFE) << 14) | (_dts[3] << 7) |
                               ((_dts[4] & 0xFE) >> 1);
                    return new TimeSpan((dts * 1111111) / 10000); // Convert 90khz to Ticks (multiply by 111.1111)
                }
                return null;
            }
        }

        // Optional Fields
        public byte[] Stuffing { get; set; }

        public PESHeader(byte[] buffer, int offset = 3)
        {
            var position = offset;

            // TODO: Check for valid marker?
            _header = buffer[position++];
            _flags = buffer[position++];
            RemainingLength = buffer[position++];
            var dataOffset = position + RemainingLength;

            if(HasPTS)
            {
                _pts = new byte[5];
                Buffer.BlockCopy(buffer, position, _pts, 0, 5);
                position += 5;
                Debug.WriteLine("Has PTS: " + this.PTS);
            }

            if(HasDTS)
            {
                _dts = new byte[5];
                Buffer.BlockCopy(buffer, position, _dts, 0, 5);
                position += 5;
                Debug.WriteLine("Has DTS: " + this.DTS);
            }

            if(HasESCR)
            {
                Debug.WriteLine("Has ESCR");
                // TODO: Load ESCR
                position += 6;
            }

            if(HasESRate)
            {
                Debug.WriteLine("Has ES Rate");
                // TODO: Load ESRate
                position += 3;
            }

            if(HasDSMTrickMode)
            {
                Debug.WriteLine("Has DSM Trick Mode");
                // TODO: Load DSMTrickMode
                position += 1;
            }

            if(HasAdditionalCopyInfo)
            {
                Debug.WriteLine("Has Additional Copyright Info");
                // TODO: Load AdditionalCopyInfo
                position += 1;
            }

            if(HasCRC)
            {
                Debug.WriteLine("Has CRC");
                // TODO: Load CRC
                position += 2;
            }

            if(HasExtension)
            {
                Debug.WriteLine("Has Extension");
                // TODO: Load PES Extension
                byte pesExtensionHeader = buffer[position++];
                bool hasPesPrivateData = (pesExtensionHeader & 0x80) > 0;

                if(hasPesPrivateData)
                {
                }
            }
        }
    }
}
