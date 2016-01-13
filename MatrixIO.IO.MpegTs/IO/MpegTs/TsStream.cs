using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MatrixIO.IO.MpegTs.Tables;
using MatrixIO.IO.MpegTs.Streams;

namespace MatrixIO.IO.MpegTs
{
    public enum StreamType : byte
    {
        Unknown = 0x00,                // ITU-T | IOS/IEC Reserved
        MPEG1_Video = 0x01,            // ISO/IEC 11172-2 Video, MPEG1
        MPEG2_Video = 0x02,            // ITU-T Rec H.262, ISO/IEC 13818-2 Video, MPEG2 or ISO/IEC 11172-2 constrained parameter video stream
        MPEG1_Audio = 0x03,            // ISO/IEC 11172 Audio
        MPEG2_Audio = 0x04,            // ISO/IEC 13818-3 Audio, MPEG2 Part 3 Audio
        MPEG2_PrivateSections = 0x05,  // ITU-T Rec. H.222.0, ISO/IEC 13818-1 private_sections, MPEG2 Part 1 Private Sections
        MPEG2_PrivateData = 0x06,      // ITU-T Rec. H.222.0, ISO/IEC 13818-1 PES packets containing private data
        MHEG_MultiMedia = 0x07,        // ISO/IEC 13522 MHEG
        MPEG2_AnnexA_DSM_CC = 0x08,    // ITU-T Rec. H.222.0, ISO/IEC 13818-1 Annex A DSM-CC
        H222_1 = 0x09,                 // ITU-T Rec. H.222.1
        MPEG2_TypeA = 0x0A,            // MPEG2 Part 6 Type A, ISO/IEC 13818-6 Type A
        MPEG2_TypeB = 0x0B,            // MPEG2 Part 6 Type B, ISO/IEC 13818-6 Type B
        MPEG2_TypeC = 0x0C,            // MPEG2 Part 6 Type C, ISO/IEC 13818-6 Type C
        MPEG2_TypeD = 0x0D,            // MPEG2 Part 6 Type D, ISO/IEC 13818-6 Type D
        MPEG2_Auxiliary = 0x0E,        // ITU-T Rec. H.220.0, ISO/IEC 13818-1 auxiliary, MPEG2 Part 1 Auxiliary 
        MPEG4_AAC = 0x0F,              // MPEG4 Part 7 AAC, ISO/IEC 13818-7:1997 AAC
        MPEG4_Visual = 0x10,           // MPEG4 Part 2 Visual a.k.a DivX/XviD, ISO/IEC 14496-2
        MPEG4_Audio = 0x11,            // MPEG4 Part 3 Audio with Amendment 1 LATM Syntax, ISO/IEC 14496-3, ISO/IEC 1
        MPEG4_FlexMuxPES = 0x12,       // ISO/IEC 14496-1 SL-packetized stream of FlexMux stream carried in PES Packets, MPEG4 Part 1
        MPEG4_FlexMuxSections = 0x13,  // ISO/IEC 14496-1 SL-packetized stream of FlexMux stream carried in ISO/IEC 14496_sections, MPEG4 Part 1
        SDP = 0x14,                    // ISO/IEC 13818-6 Synchronized Download Protocol
        METADATA_PES = 0x15,           // Metadata carried in PES packets
        METADATA_Sections = 0x16,      // Metadata carried in metadata_sections
        METADATA_DataCarousel = 0x17,  // Metadata carried in ISO/IEC 13818-6 Data Carousel
        METADATA_ObjectCarousel = 0x18,// Metadata carried in ISO/IEC 13818-6 Object Carousel
        METADATA_SDP = 0x19,           // Metadata carried in ISO/IEC 13818-6 Synchronized Download Protocol
        IPMP = 0x1A,                   // IPMP stream (defined in ISO/IEC 13818-11, MPEG-2 IPMP)
        MPEG4_AVC = 0x1B,              // ITU-T h.264, MPEG4 Part 10 AVC, ISO/IEC 14496-10 AVC
        // 0x1C - 0x7F ITU-T Rec. H.222.0 | ISO/IEC 13818-1 Reserved
        // 0x7f is sometimes used for IPMP streams as well.
        // 0x80 - 0xFF USER PRIVATE

        // Blu-Ray BDAV specific types
        // TODO: I'm not sure these are in the correct order or what the 3 numbered ones are.  AC3 and VC1 are the only one I've verified.
        BDAV_Audio_LPCM = 0x80,
        BDAV_Audio_AC3 = 0x81,
        BDAV_Audio_DDPlus = 0x82,
        BDAV_Audio_DolbyLossless = 0x83,
        BDAV_Audio_DTS = 0x84,
        BDAV_Audio_DTSHD = 0x85,
        BDAV_Audio_7 = 0x86,
        BDAV_Audio_8 = 0xA1,
        BDAV_Audio_9 = 0xA2,
        BDAV_VIDEO_VC1 = 0xEA, // Defined for Blu-Ray
    }

    public abstract class TsStream : INotifyPropertyChanged
    {

        public event EventHandler<TsStreamEventArgs> UnitReceived;
        public void OnUnitReceived(TsStreamEventArgs e)
        {
            if (UnitReceived != null)
                UnitReceived(this, e);
        }

        private ushort _packetIdentifier;
        public ushort PacketIdentifier
        {
            get { return _packetIdentifier; }
            set { _packetIdentifier = value; OnPropertyChanged("PacketIdentifier"); }
        }

        private StreamType _type;
        public StreamType Type
        {
            get { return _type; }
            set { _type = value; OnPropertyChanged("Type"); }
        }

        private IList<TsDescriptor> _info;
        public IList<TsDescriptor> Info
        {
            get { return _info; }
            set { _info = value; OnPropertyChanged("Info"); }
        }

        private long _droppedPackets;
        public long DroppedPackets
        {
            get { return _droppedPackets; }
            protected set { _droppedPackets = value; OnPropertyChanged("DroppedPackets"); }
        }

        private long _skippedPackets;
        public long SkippedPackets
        {
            get { return _skippedPackets; }
            protected set { _skippedPackets = value; OnPropertyChanged("SkippedPackets"); }
        }

        public abstract void ProcessInput(TsPacket packet);

        protected static int EstimateDroppedPackets(int lastCounter, int currentCounter)
        {
            if (lastCounter < currentCounter) return currentCounter - lastCounter - 1;
            if (lastCounter > currentCounter) return 15 - lastCounter + currentCounter;
            return 0;
        }

        public override string ToString()
        {
            return Type + " (PID " + PacketIdentifier + ")";
        }

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) 
                Portability.DispatchAction(() => PropertyChanged(this, new PropertyChangedEventArgs(propertyName)));
        }
        #endregion

        public static Dictionary<StreamType, Type> StreamTypes = new Dictionary<StreamType,Type>()
        {
            { StreamType.MPEG1_Video, typeof(PacketizedElementalStream) },
            { StreamType.MPEG2_Video, typeof(PacketizedElementalStream) },
            { StreamType.MPEG1_Audio, typeof(PacketizedElementalStream) },
            { StreamType.MPEG2_Audio, typeof(PacketizedElementalStream) },
            { StreamType.MPEG2_PrivateSections, typeof(ByteStream) }, // Are these related to Table Sections?
            { StreamType.MPEG2_PrivateData, typeof(PacketizedElementalStream) },
            { StreamType.MHEG_MultiMedia, typeof(ByteStream) }, // ?
            { StreamType.MPEG2_AnnexA_DSM_CC, typeof(ByteStream) }, // ?
            { StreamType.H222_1, typeof(ByteStream) }, // ?
            { StreamType.MPEG2_TypeA, typeof(ByteStream) }, // ?
            { StreamType.MPEG2_TypeB, typeof(ByteStream) }, // ?
            { StreamType.MPEG2_TypeC, typeof(ByteStream) }, // ?
            { StreamType.MPEG2_TypeD, typeof(ByteStream) }, // ?
            { StreamType.MPEG2_Auxiliary, typeof(ByteStream) }, // ?
            { StreamType.MPEG4_AAC, typeof(PacketizedElementalStream) }, // ?
            { StreamType.MPEG4_Visual, typeof(ByteStream) }, // ?
            { StreamType.MPEG4_Audio, typeof(ByteStream) }, // ?
            { StreamType.MPEG4_FlexMuxPES, typeof(ByteStream) }, // ?
            { StreamType.MPEG4_FlexMuxSections, typeof(ByteStream) }, // ?
            { StreamType.SDP, typeof(ByteStream) }, // ?
            { StreamType.MPEG4_AVC, typeof(PacketizedElementalStream) }, // ?
        };
    }

    public abstract class TsStream<T> : TsStream
        where T: class
    {
        private byte _lastContinuityCounter;
        private bool _unitInProgress;
        private long _unitCount;
        private TsUnit _currentUnit = new TsUnit();

        public override void ProcessInput(TsPacket packet)
        {
            byte expectedContinuityCounter = packet.Payload != null ? (byte)(_lastContinuityCounter + 1) : _lastContinuityCounter;
            if (expectedContinuityCounter > 15) expectedContinuityCounter -= 16;

            if (packet.PayloadUnitStartIndicator)
            {
                if (_unitInProgress)
                {
#if DEBUG
                    var adaptationfields = (from p in _currentUnit.Packets where p.AdaptationField != null select p.AdaptationField).Count();
                    Debug.WriteLine("### Completed unit for PID " + (PacketIdentifier) PacketIdentifier + " with " +
                                    _currentUnit.Packets.Count
                                    + " packets, " + _currentUnit.Payload.Count + " bytes of payload and " +
                                    adaptationfields + " adaptation fields.");
#endif
                    var unit = ProcessUnit(_currentUnit);
                    if(unit!=null) OnUnitReceived(new TsStreamEventArgs<T>(PacketIdentifier, _currentUnit, unit));
                    _unitInProgress = false;
                    _currentUnit = new TsUnit();
                }
                _currentUnit.Packets.Add(packet);

                if (_unitCount > 0 && packet.ContinuityCounter != expectedContinuityCounter)
                {
                    int droppedPackets = EstimateDroppedPackets(_lastContinuityCounter, packet.ContinuityCounter);
                    Debug.WriteLine("*** Discontinuity (0) - Dropped " + droppedPackets + " packets. ***");
                }

                _lastContinuityCounter = packet.ContinuityCounter;
                _unitInProgress = true;
                _unitCount++;
            }
            else if (_unitInProgress)
            {
                if (packet.ContinuityCounter == expectedContinuityCounter)
                {
                    _currentUnit.Packets.Add(packet);

                    _lastContinuityCounter = packet.ContinuityCounter;
                    if (packet.Payload != null) _lastContinuityCounter = packet.ContinuityCounter;

                    // Check to see if this is the last packet
                }
                else
                {
                    var droppedPackets = EstimateDroppedPackets(_lastContinuityCounter, packet.ContinuityCounter);
                    DroppedPackets += droppedPackets;
                    Debug.WriteLine("*** Discontinuity (1) - Dropped " + droppedPackets + " packets. ***");
                    _unitInProgress = false;
                    _currentUnit.Packets.Clear();
                }
            }
            else if (_unitCount > 0)
            {
                // This should be the result of a prior Discontinuity (1) as we just have to 
                // drop the remaining packets until the next unit start packet.
                SkippedPackets++;
                Debug.WriteLine("*** Skipping packet due to prior discontinuity. ***");
            }
            else
            {
                // Silently drop any packets that arrive before our first unit start packet.
                SkippedPackets++;
                Debug.WriteLine("*** Skipping initial packet before first Unit Start. ***");
            }
        }

        protected abstract T ProcessUnit(TsUnit unit);

        public void OnUnitReceived(TsStreamEventArgs<T> e)
        {
            base.OnUnitReceived(e);
        }
    }

    public class TsStreamEventArgs : EventArgs
    {
        public ushort PacketIdentifier { get; private set; }
        public TsUnit Unit { get; private set; }

        public TsStreamEventArgs(ushort packetIdentifier, TsUnit unit)
        {
            PacketIdentifier = packetIdentifier;
            Unit = unit;
        }
    }

    public class TsStreamEventArgs<T> : TsStreamEventArgs
    {
        public T DecodedUnit { get; private set; }

        public TsStreamEventArgs(ushort packetIdentifier, TsUnit unit, T decodedUnit) : base(packetIdentifier, unit)
        {
            DecodedUnit = decodedUnit;
        }
    }
}
