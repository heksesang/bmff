using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Track Extends Box ("trex")
    /// </summary>
    [Box("trex", "Track Extends Box")]
    public class TrackExtendsBox : FullBox
    {
        public TrackExtendsBox() : base() { }
        public TrackExtendsBox(Stream stream) : base(stream) { }

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + (5 * 4);
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            TrackID = stream.ReadBEUInt32();
            DefaultSampleDescriptionIndex = stream.ReadBEUInt32();
            DefaultSampleDuration = stream.ReadBEUInt32();
            DefaultSampleSize = stream.ReadBEUInt32();
            DefaultSampleFlags._flags = stream.ReadBEUInt32();
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            stream.WriteBEUInt32(TrackID);
            stream.WriteBEUInt32(DefaultSampleDescriptionIndex);
            stream.WriteBEUInt32(DefaultSampleDuration);
            stream.WriteBEUInt32(DefaultSampleSize);
            stream.WriteBEUInt32(DefaultSampleFlags._flags);
        }

        public uint TrackID { get; set; }
        public uint DefaultSampleDescriptionIndex { get; set; }
        public uint DefaultSampleDuration { get; set; }
        public uint DefaultSampleSize { get; set; }
        private SampleFlags DefaultSampleFlags = new SampleFlags();

        public byte DefaultSampleDependsOn
        {
            get
            {
                return DefaultSampleFlags.SampleDependsOn;
            }
            set
            {
                DefaultSampleFlags.SampleDependsOn = value;
            }
        }

        public byte DefaultSampleIsDependedOn
        {
            get
            {
                return DefaultSampleFlags.SampleIsDependedOn;
            }
            set
            {
                DefaultSampleFlags.SampleIsDependedOn = value;
            }
        }

        public byte DefaultSampleHasRedundancy
        {
            get
            {
                return DefaultSampleFlags.SampleHasRedundancy;
            }
            set
            {
                DefaultSampleFlags.SampleHasRedundancy = value;
            }
        }

        public byte DefaultSamplePaddingValue
        {
            get
            {
                return DefaultSampleFlags.SamplePaddingValue;
            }
            set
            {
                DefaultSampleFlags.SamplePaddingValue = value;
            }
        }

        public bool DefaultSampleIsDifferenceValue
        {
            get
            {
                return DefaultSampleFlags.SampleIsDifferenceValue;
            }
            set
            {
                DefaultSampleFlags.SampleIsDifferenceValue = value;
            }
        }

        public ushort DefaultDegredationPriority
        {
            get
            {
                return DefaultSampleFlags.DegredationPriority;
            }
            set
            {
                DefaultSampleFlags.DegredationPriority = value;
            }
        }
    }

    public class SampleFlags
    {
        internal uint _flags;

        public SampleFlags() {}
        public SampleFlags(uint value)
        {
            _flags = value;
        }

        public byte SampleDependsOn // 2 bits -- Defined in Independent and Disposable Samples Box
        {
            get
            {
                return (byte)((_flags & 0x3000000) >> 24);
            }
            set
            {
                if (value > 3) throw new ArgumentOutOfRangeException("SampleDependsOn is a 2 bit field and only accepts values 0 through 4.");
                _flags = (_flags & 0xFCFFFFFF) | (((uint)value) << 24);
            }
        }

        public byte SampleIsDependedOn // 2 bits -- Defined in Independent and Disposable Samples Box
        {
            get
            {
                return (byte)((_flags & 0x00C00000) >> 22);
            }
            set
            {
                if (value > 3) throw new ArgumentOutOfRangeException("SampleIsDependedOn is a 2 bit field and only accepts values 0 through 4.");
                _flags = (_flags & 0xFF3FFFFF) | (((uint)value) << 22);
            }
        }

        public byte SampleHasRedundancy // 2 bits -- Defined in Independent and Disposable Samples Box
        {
            get
            {
                return (byte)((_flags & 0x00300000) >> 20);
            }
            set
            {
                if (value > 3) throw new ArgumentOutOfRangeException("SampleHasRedundancy is a 2 bit field and only accepts values 0 through 4.");
                _flags = (_flags & 0xFFCFFFFF) | (((uint)value) << 20);
            }
        }

        public byte SamplePaddingValue // 3 bits -- Defined in degredation priority table
        {
            get
            {
                return (byte)((_flags & 0x000E0000) >> 17);
            }
            set
            {
                if (value > 7) throw new ArgumentOutOfRangeException("SamplePaddingValue is a 3 bit field and only accepts values 0 through 7.");
                _flags = (_flags & 0xFFF1FFFF) | (((uint)value) << 17);
            }
        }

        public bool SampleIsDifferenceValue // 1 bit
        {
            get
            {
                return (_flags & 0x00010000) == 0x00010000;
            }
            set
            {
                _flags |= 0x00010000;
            }
        }

        public ushort DegredationPriority // 16 bits
        {
            get
            {
                return (ushort)(_flags & 0x0000FFFF);
            }
            set
            {
                _flags = (_flags & 0xFFFF0000) | value;
            }
        }
    }
}
