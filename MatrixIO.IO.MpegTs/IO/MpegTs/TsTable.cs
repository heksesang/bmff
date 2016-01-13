using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace MatrixIO.IO.MpegTs
{
    public enum TableIdentifier : byte
    {
        ProgramAssociation = 0x00,
        ConditionalAccess = 0x01,
        ProgramMap = 0x02,
        Description = 0x03,
        SceneDescriptor = 0x04,
        ObjectDescriptor = 0x05,
        // 0x06 to 0x37 RESERVED
        // 0x38 to 0x39 ISO/IEC 13818-6 reserved
        NetworkInformation = 0x40,
        OtherNetworkInformation = 0x41,
        ServiceDescription = 0x42,
        // 0x43 to 0x45 RESERVED 
        ServiceDescriptionOther = 0x46,
        // 0x47 to 0x49 RESERVED
        BouquetAssociation = 0x4A,
        // 0x4B to 0x4D RESERVED
        EventInformation = 0x4E,
        EventInformationOther = 0x4F,
        EventInformationSchedule0 = 0x50,
        EventInformationSchedule1 = 0x51,
        EventInformationSchedule2 = 0x52,
        EventInformationSchedule3 = 0x53,
        EventInformationSchedule4 = 0x54,
        EventInformationSchedule5 = 0x55,
        EventInformationSchedule6 = 0x56,
        EventInformationSchedule7 = 0x57,
        EventInformationSchedule8 = 0x58,
        EventInformationSchedule9 = 0x59,
        EventInformationScheduleA = 0x5A,
        EventInformationScheduleB = 0x5B,
        EventInformationScheduleC = 0x5C,
        EventInformationScheduleD = 0x5D,
        EventInformationScheduleE = 0x5E,
        EventInformationScheduleF = 0x5F,
        EventInformationScheduleOther0 = 0x60,
        EventInformationScheduleOther1 = 0x61,
        EventInformationScheduleOther2 = 0x62,
        EventInformationScheduleOther3 = 0x63,
        EventInformationScheduleOther4 = 0x64,
        EventInformationScheduleOther5 = 0x65,
        EventInformationScheduleOther6 = 0x66,
        EventInformationScheduleOther7 = 0x67,
        EventInformationScheduleOther8 = 0x68,
        EventInformationScheduleOther9 = 0x69,
        EventInformationScheduleOtherA = 0x6A,
        EventInformationScheduleOtherB = 0x6B,
        EventInformationScheduleOtherC = 0x6C,
        EventInformationScheduleOtherD = 0x6D,
        EventInformationScheduleOtherE = 0x6E,
        EventInformationScheduleOtherF = 0x6F,
        TimeAndDate = 0x70,
        RunningStatus = 0x71,
        Stuffing = 0x72,
        TimeOffset = 0x73,
        ApplicationInformation = 0x74,
        Container = 0x75,
        RelatedContent = 0x76,
        ContentIdentification = 0x77,
        FowardErrorCorrection = 0x78,
        ResolutionNotification = 0x79,
        InterburstForwardErrorCorrection = 0x7A,
        // 0x7B to 0x7D RESERVED
        DiscontinuityInformation = 0x7E,
        SectionInformation = 0x7F,
        // 0x80 to 0xFE USER DEFINED
        // 0xFF RESERVED
    }
    public abstract class TsTable
    {
        public TableIdentifier TableIdentifier { get; set; } 

    }

    public abstract class TsTable<T>: TsTable, IEnumerable<T>
    {
        protected int _position;

        private ushort _sectionInfo = 0x3000;
        public bool SectionSyntaxIndicator
        {
            get { return (_sectionInfo & 0x8000) != 0; }
            set { if (value) _sectionInfo |= 0x8000; else _sectionInfo &= 0x7FFF; }
        }
        
        // Reserved 3 bits - always 011

        public ushort SectionLength
        {
            get { return (ushort)(_sectionInfo & 0x0FFF); }
            // Max value should be 0x3FD (1021)
            set { _sectionInfo = (ushort)((_sectionInfo & 0xF000) | (value & 0x0FFF)); }
        }

        protected int _sectionLengthStartOffset;

        protected ushort _identifier;
        protected byte _versionInfo; // 2 bit reserved, 5 bit version, 1 bit current next indicator

        public byte Version
        {
            get { return (byte)((_versionInfo & 0x3E) >> 1); }
            set { _versionInfo = (byte)((_versionInfo & 0xC1) | ((value & 0x1F) << 1)); }
        }

        public byte SectionNumber { get; set; }
        public byte LastSectionNumber { get; set; }

        public bool IsCurrent
        {
            get { return (_versionInfo & 0x01) > 0; }
            set { _versionInfo = (byte)((_versionInfo & 0xFE) | (value ? 0x01 : 0x00)); }
        }

        public IList<T> Rows { get; private set; }

        public T this[int i] { get { return Rows[i]; } }

        protected TsTable()
        {
            Rows = Portability.CreateList<T>();
        }
        protected TsTable(byte[] buffer, int offset, int length) : this()
        {
            _position = offset;

            TableIdentifier = (TableIdentifier)buffer[_position++];
            _sectionInfo = (ushort)((buffer[_position++] << 8) | buffer[_position++]);
            _sectionLengthStartOffset = _position;
            Debug.WriteLine("SectionInfo: 0x" + _sectionInfo.ToString("X4") + " SectionLength: 0x" + SectionLength.ToString("X4"));

            uint crc = BitConverter.ToUInt32(buffer, _position + SectionLength - 4);
            uint computedCRC = ComputeCRC(buffer, offset , (_position - offset) + SectionLength - 4);
            if (crc != computedCRC) throw new ArgumentException("Invalid table CRC");

            _identifier = (ushort)((buffer[_position++] << 8) | buffer[_position++]);
            Debug.WriteLine("TS Identifier: 0x" + _identifier.ToString("X4"));
            _versionInfo = buffer[_position++];
            SectionNumber = buffer[_position++];
            LastSectionNumber = buffer[_position++];
            Debug.WriteLine("SectionLength=" + SectionLength + " TableIdentifier=" + TableIdentifier + " SectionNumber=" + SectionNumber + " LastSectionNumber=" + LastSectionNumber);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return ((IEnumerable<T>)Rows).GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return Rows.GetEnumerator();
        }

        private static uint ComputeCRC(byte[] buffer, int offset, int count)
        {
            uint crc = 0xFFFFFFFF;
            for (int i = offset; i < offset+count; i++)
                crc = CRC32Table[unchecked((byte)crc) ^ buffer[i]] ^ (crc >> 8);
            return crc;
        }

        private static readonly uint[] CRC32Table =
            {
                0x00000000, 0xB71DC104, 0x6E3B8209, 0xD926430D,
                0xDC760413, 0x6B6BC517, 0xB24D861A, 0x0550471E,
                0xB8ED0826, 0x0FF0C922, 0xD6D68A2F, 0x61CB4B2B,
                0x649B0C35, 0xD386CD31, 0x0AA08E3C, 0xBDBD4F38,
                0x70DB114C, 0xC7C6D048, 0x1EE09345, 0xA9FD5241,
                0xACAD155F, 0x1BB0D45B, 0xC2969756, 0x758B5652,
                0xC836196A, 0x7F2BD86E, 0xA60D9B63, 0x11105A67,
                0x14401D79, 0xA35DDC7D, 0x7A7B9F70, 0xCD665E74,
                0xE0B62398, 0x57ABE29C, 0x8E8DA191, 0x39906095,
                0x3CC0278B, 0x8BDDE68F, 0x52FBA582, 0xE5E66486,
                0x585B2BBE, 0xEF46EABA, 0x3660A9B7, 0x817D68B3,
                0x842D2FAD, 0x3330EEA9, 0xEA16ADA4, 0x5D0B6CA0,
                0x906D32D4, 0x2770F3D0, 0xFE56B0DD, 0x494B71D9,
                0x4C1B36C7, 0xFB06F7C3, 0x2220B4CE, 0x953D75CA,
                0x28803AF2, 0x9F9DFBF6, 0x46BBB8FB, 0xF1A679FF,
                0xF4F63EE1, 0x43EBFFE5, 0x9ACDBCE8, 0x2DD07DEC,
                0x77708634, 0xC06D4730, 0x194B043D, 0xAE56C539,
                0xAB068227, 0x1C1B4323, 0xC53D002E, 0x7220C12A,
                0xCF9D8E12, 0x78804F16, 0xA1A60C1B, 0x16BBCD1F,
                0x13EB8A01, 0xA4F64B05, 0x7DD00808, 0xCACDC90C,
                0x07AB9778, 0xB0B6567C, 0x69901571, 0xDE8DD475,
                0xDBDD936B, 0x6CC0526F, 0xB5E61162, 0x02FBD066,
                0xBF469F5E, 0x085B5E5A, 0xD17D1D57, 0x6660DC53,
                0x63309B4D, 0xD42D5A49, 0x0D0B1944, 0xBA16D840,
                0x97C6A5AC, 0x20DB64A8, 0xF9FD27A5, 0x4EE0E6A1,
                0x4BB0A1BF, 0xFCAD60BB, 0x258B23B6, 0x9296E2B2,
                0x2F2BAD8A, 0x98366C8E, 0x41102F83, 0xF60DEE87,
                0xF35DA999, 0x4440689D, 0x9D662B90, 0x2A7BEA94,
                0xE71DB4E0, 0x500075E4, 0x892636E9, 0x3E3BF7ED,
                0x3B6BB0F3, 0x8C7671F7, 0x555032FA, 0xE24DF3FE,
                0x5FF0BCC6, 0xE8ED7DC2, 0x31CB3ECF, 0x86D6FFCB,
                0x8386B8D5, 0x349B79D1, 0xEDBD3ADC, 0x5AA0FBD8,
                0xEEE00C69, 0x59FDCD6D, 0x80DB8E60, 0x37C64F64,
                0x3296087A, 0x858BC97E, 0x5CAD8A73, 0xEBB04B77,
                0x560D044F, 0xE110C54B, 0x38368646, 0x8F2B4742,
                0x8A7B005C, 0x3D66C158, 0xE4408255, 0x535D4351,
                0x9E3B1D25, 0x2926DC21, 0xF0009F2C, 0x471D5E28,
                0x424D1936, 0xF550D832, 0x2C769B3F, 0x9B6B5A3B,
                0x26D61503, 0x91CBD407, 0x48ED970A, 0xFFF0560E,
                0xFAA01110, 0x4DBDD014, 0x949B9319, 0x2386521D,
                0x0E562FF1, 0xB94BEEF5, 0x606DADF8, 0xD7706CFC,
                0xD2202BE2, 0x653DEAE6, 0xBC1BA9EB, 0x0B0668EF,
                0xB6BB27D7, 0x01A6E6D3, 0xD880A5DE, 0x6F9D64DA,
                0x6ACD23C4, 0xDDD0E2C0, 0x04F6A1CD, 0xB3EB60C9,
                0x7E8D3EBD, 0xC990FFB9, 0x10B6BCB4, 0xA7AB7DB0,
                0xA2FB3AAE, 0x15E6FBAA, 0xCCC0B8A7, 0x7BDD79A3,
                0xC660369B, 0x717DF79F, 0xA85BB492, 0x1F467596,
                0x1A163288, 0xAD0BF38C, 0x742DB081, 0xC3307185,
                0x99908A5D, 0x2E8D4B59, 0xF7AB0854, 0x40B6C950,
                0x45E68E4E, 0xF2FB4F4A, 0x2BDD0C47, 0x9CC0CD43,
                0x217D827B, 0x9660437F, 0x4F460072, 0xF85BC176,
                0xFD0B8668, 0x4A16476C, 0x93300461, 0x242DC565,
                0xE94B9B11, 0x5E565A15, 0x87701918, 0x306DD81C,
                0x353D9F02, 0x82205E06, 0x5B061D0B, 0xEC1BDC0F,
                0x51A69337, 0xE6BB5233, 0x3F9D113E, 0x8880D03A,
                0x8DD09724, 0x3ACD5620, 0xE3EB152D, 0x54F6D429,
                0x7926A9C5, 0xCE3B68C1, 0x171D2BCC, 0xA000EAC8,
                0xA550ADD6, 0x124D6CD2, 0xCB6B2FDF, 0x7C76EEDB,
                0xC1CBA1E3, 0x76D660E7, 0xAFF023EA, 0x18EDE2EE,
                0x1DBDA5F0, 0xAAA064F4, 0x738627F9, 0xC49BE6FD,
                0x09FDB889, 0xBEE0798D, 0x67C63A80, 0xD0DBFB84,
                0xD58BBC9A, 0x62967D9E, 0xBBB03E93, 0x0CADFF97,
                0xB110B0AF, 0x060D71AB, 0xDF2B32A6, 0x6836F3A2,
                0x6D66B4BC, 0xDA7B75B8, 0x035D36B5, 0xB440F7B1
            };
    }
}
