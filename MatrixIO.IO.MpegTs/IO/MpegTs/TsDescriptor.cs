using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace MatrixIO.IO.MpegTs
{
    public enum DescriptorTag : byte
    {
        // 0-1 RESERVED
        VideoStream = 2,
        AudioStream = 3,
        Hierarchy = 4,
        Registration = 5,
        DataStreamAlignment = 6,
        TargetBackgroundGrid = 7,
        VideoWindow = 8,
        CA = 9,
        Iso639Language = 10,
        SystemClock = 11,
        MultiplexBufferUtilization = 12,
        Copyright = 13,
        MaximumBitrate = 14,
        PrivateDataIndicator = 15,
        SmoothingBuffer = 16,
        STD = 17,
        IBP = 18,
        // 19-26 Defined in ISO/IEC 13818-6
        AssociationTag = 20,
        //
        MPEG4Video = 27,
        MPEG4Audio = 28,
        IOD = 29,
        SL = 30,
        FMC = 31,
        ExternalEsId = 32,
        MuxCode = 33,
        FmxBufferSize = 34,
        MultiplexBuffer = 35,
        FlexMuxTiming = 36,
        // 37-63 ITU-T Rec. H.222.0 | ISO/IEC 13818-1 RESERVED
        // 64-255 User Private

        // DVB Descriptors
        DVB_NetworkName = 0x40,
        DVB_ServiceList = 0x41,
        DVB_Stuffing = 0x42,
        DVB_SatelliteDeliverySystem = 0x43,
        DVB_CableDeliverySystem = 0x44,
        DVB_VBIData = 0x45,
        DVB_VBITeletext = 0x46,
        DVB_BouquetName = 0x47,
        DVB_Service = 0x48,
        DVB_CountryAvailability = 0x49,
        DVB_Linkage = 0x4A,
        DVB_NVODReference = 0x4B,
        DVB_TimeShiftedService = 0x4C,
        DVB_ShortEvent = 0x4D,
        DVB_ExtendedEvent = 0x4E,
        DVB_TimeShiftedEvent = 0x4F,
        DVB_Component = 0x50,
        DVB_Mosaic = 0x51,
        DVB_StreamIdentifier = 0x52,
        DVB_CAIdentifier = 0x53,
        DVB_Content = 0x54,
        DVB_ParentalRating = 0x55,
        DVB_Teletext = 0x56,
        DVB_Telephone = 0x57,
        DVB_LocalTimeOffset = 0x58,
        DVB_Subtitling = 0x59,
        DVB_TerrestrialDeliverySystem = 0x5A,
        DVB_MultilingualNetworkName = 0x5B,
        DVB_MultilingualBouquetName = 0x5C,
        DVB_MultilingualServiceName = 0x5D,
        DVB_MultilingualComponentName = 0x5E,
        DVB_PrivateDataSpecifier = 0x5F,
        DVB_ServiceMode = 0x60,
        DVB_ShortSmoothingBuffer = 0x61,
        DVB_FrequencyList = 0x62,
        DVB_PartialTransportStream = 0x63,
        DVB_DataBroadcast = 0x64,
        DVB_CASystem = 0x65,
        DVB_DataBroadcastId = 0x66,
        DVB_TransportStream = 0x67,
        DVB_DSNG = 0x68,
        DVB_PDC = 0x69,
        DVB_AC3 = 0x6A,
        DVB_AncillaryData = 0x6B,
        DVB_CellList = 0x6C,
        DVB_CellFrequencyLink = 0x6D,
        DVB_AnnouncementSupport = 0x6E,

        // ATSC Descriptors
        ATSC_Stuffing = 0x80,
        ATSC_AC3 = 0x81,
        ATSC_TimeShiftedService = 0x82,
        ATSC_CaptionService = 0x86,
        ATSC_ContentAdvisory = 0x87,
        ATSC_DCCArrivingRequest = 0x89,
        ATSC_ExtendedChannelName = 0xA0,
        ATSC_ServiceLocation = 0xA1,
        ATSC_ComponentName = 0xA3,
        ATSC_DataService = 0xA4,
        ATSC_PIDCount = 0xA5,
        ATSC_Download = 0xA6,
        ATSC_MultiprotocolEncapsulation = 0xA7,
        ATSC_DCCDepartingRequest = 0xA8,
        ATSC_RedistributionControl = 0xAA,
        ATSC_Genre = 0xAB,
        ATSC_PrivateInformation = 0xAD,
        ATSC_ContentIdentifier = 0xB6,
    }

    public abstract class TsDescriptor
    {
        protected int _position;
        protected int _length;

        public DescriptorTag Tag { get; set; }
        public virtual int Length { get { return 2 + _length; } protected set { _length = value - 2; } }

        protected TsDescriptor(byte[] buffer, int offset)
        {
            _position = offset;
            Tag = (DescriptorTag) buffer[_position++];
            _length = buffer[_position++];

            Debug.WriteLine("Loading Descriptor: " + Tag);
        }
    }
}
