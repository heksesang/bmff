using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using MatrixIO.IO.MpegTs.Streams;

namespace MatrixIO.IO.MpegTs
{
    public abstract class TsStream : INotifyPropertyChanged
    {
        public event EventHandler<TsStreamEventArgs> UnitReceived;
        public void OnUnitReceived(TsStreamEventArgs e)
        {
            UnitReceived?.Invoke(this, e);
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
