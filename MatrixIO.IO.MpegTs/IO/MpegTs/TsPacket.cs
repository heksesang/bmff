using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MatrixIO.IO.MpegTs
{
    public enum ScramblingControl : byte
    {
        NotScambled = 0,
        Reserved = 1,
        ScrambledWithEvenKey = 2,
        ScambledWithOddKey = 3,
    }

    public enum PacketIdentifier : ushort
    {
        ProgramAssociationTable         = 0x0000, // TID 0x00
        ConditionalAccessTable          = 0x0001, // TID 0x01
        TsDescriptionTable              = 0x0002, // TID 0x03
        // 0x03 to 0x0F RESERVED
        NetworkInformationTable         = 0x0010, // TID 0x40 (current) & 0x41 (other)
        ServiceDescriptionTable         = 0x0011, // TID 0x42,0x46 (ServiceDescription) & 0x4A (BouquetAssociation)
        EventInformationTable           = 0x0012,
        RunningStatusTable              = 0x0013,
        TimeTables                      = 0x0014, // TID 0x70 (TimeAndDate) & 0x73 (TimeOffset)
        NetworkSynchronization          = 0x0015,
        ResolutionNotificationTable     = 0x0016,
        // 0x0017 to 0x001B RESERVED
        InbandSignalling                = 0x001C,
        Measurement                     = 0x001D,
        DiscontinuityInformation        = 0x001E,
        SectionIformation               = 0x001F,
        // USER DEFINED
        NullPacket                      = 0x1FFF,
    }

    public class TsPacket
    {
        public const int Length = 188;
        internal const byte SyncByte = 0x47;

        private ushort _header1;
        private byte _header2;

        public bool TransportErrorIndicator
        {
            get { return (_header1 & 0x8000) != 0; }
            set { if (value) _header1 |= 0x8000; else _header1 &= 0x7FFF; }
        }

        public bool PayloadUnitStartIndicator
        {
            get { return (_header1 & 0x4000) != 0; }
            set { if (value) _header1 |= 0x4000; else _header1 &= 0xBFFF; }
        }

        public bool TransportPriority
        {
            get { return (_header1 & 0x2000) != 0; }
            set { if (value) _header1 |= 0x2000; else _header1 &= 0xDFFF; }
        }

        /// <summary>
        /// 13 bit identifier.  Valid values are 0 to 8191.
        /// </summary>
        public ushort PacketIdentifier
        {
            get { return (ushort)(_header1 & 0x1FFF); }
            set
            {
                Debug.Assert(value < 8192, "PacketIdentifier must be in the range of 0 to 8191.");
                _header1 = (ushort)((_header1 & 0xE000) | (value & 0x1FFF));
            }
        }

        public ScramblingControl ScramblingControl
        {
            get { return (ScramblingControl)((_header2 & 0xC0) >> 6); }
            set 
            { 
                Debug.Assert((int)value < 4, "ScramblingControl must be in the range of 0 to 3.");
                _header2 = (byte)((_header2 & 0x3F) | (((byte)value << 6) & 0xC0));
            }
        }

        private bool HasAdaptationField
        {
            get { return (_header2 & 0x20) != 0; }
            set { if (value) _header2 |= 0x20; else _header2 &= 0xDF; }
        }
        private bool HasPayload
        {
            get { return (_header2 & 0x10) != 0; }
            set { if (value) _header2 |= 0x10; else _header2 &= 0xEF; }
        }

        /// <summary>
        /// 4 bit counter. Valid values are 0 to 15.
        /// </summary>
        public byte ContinuityCounter
        {
            get { return (byte)(_header2 & 0x0F); }
            set
            {
                Debug.Assert((int) value < 16, "ContinuityCounter must be in the range of 0 to 15.");
                _header2 = (byte)((_header2 & 0xF0) | (value & 0x0F));
            }
        }

        private AdaptationField _adaptationField;
        public AdaptationField AdaptationField
        {
            get
            {
                return HasAdaptationField ? _adaptationField : null;    
            }
            set
            {
                _adaptationField = value;
                HasAdaptationField = (value != null);
            }
        }

        private byte[] _payload;
        public byte[] Payload
        {
            get { return HasPayload ? _payload : null; }
            set
            {
                _payload = value;
                HasPayload = (value != null);
            }
        }

        public byte[] Stuffing { get; set; }

        public TsPacket(byte[] buffer, int offset=0)
        {
            int position = offset;

            if (buffer.Length - offset < Length) 
                throw new ArgumentOutOfRangeException("Buffer must contain at least " + Length + " bytes.");
            if (buffer[position++] != SyncByte) 
                throw new ArgumentException("Packet does not begin with 0x" + SyncByte.ToString("X2") + " sync byte.");
            _header1 = (ushort)((buffer[position++] << 8) | buffer[position++]);
            _header2 = buffer[position++];

            if(HasAdaptationField)
            {
                _adaptationField = new AdaptationField(buffer, position);
                position += AdaptationField.Length + 1;
            }

            if (HasPayload)
            {
                int payloadLength = Length - (position - offset);
                _payload = new byte[payloadLength];
                Buffer.BlockCopy(buffer, position, _payload, 0, payloadLength);
                position += payloadLength;
            }
            int stuffingLength = Length - (position - offset);
            if (stuffingLength > 0)
            {
                Debug.WriteLine("Has " + stuffingLength + " bytes of Transport Packet stuffing.");
                Stuffing = new byte[stuffingLength];
            }
        }
    }
}
