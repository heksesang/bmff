using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Track Fragment Header Box ("tfhd")
    /// </summary>
    [Box("tfhd", "Track Fragment Header Box")]
    public class TrackFragmentHeaderBox : FullBox
    {

        public TrackFragmentHeaderBox() : base() { }
        public TrackFragmentHeaderBox(Stream stream) : base(stream) { }

        internal override ulong CalculateSize()
        {
                ulong calculatedSize = base.CalculateSize() + 4;

                if (BaseDataOffset.HasValue) calculatedSize += 8;
                if (SampleDescriptionIndex.HasValue) calculatedSize += 4;
                if (DefaultSampleDuration.HasValue) calculatedSize += 4;
                if (DefaultSampleSize.HasValue) calculatedSize += 4;
                if (DefaultSampleFlags != null) calculatedSize += 4;

                return calculatedSize;
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            TrackID = stream.ReadBEUInt32();

            if ((Flags & TrackFragmentFlags.BaseDataOffsetPresent) == TrackFragmentFlags.BaseDataOffsetPresent)
                BaseDataOffset = stream.ReadBEUInt64();
            if ((Flags & TrackFragmentFlags.SampleDrescriptionIndexPresent) == TrackFragmentFlags.SampleDrescriptionIndexPresent)
                SampleDescriptionIndex = stream.ReadBEUInt32();
            if ((Flags & TrackFragmentFlags.DefaultSampleDurationPresent) == TrackFragmentFlags.DefaultSampleDurationPresent)
                DefaultSampleDuration = stream.ReadBEUInt32();
            if ((Flags & TrackFragmentFlags.DefaultSampleSizePresent) == TrackFragmentFlags.DefaultSampleSizePresent)
                DefaultSampleSize = stream.ReadBEUInt32();
            if ((Flags & TrackFragmentFlags.DefaultSampleFlagsPresent) == TrackFragmentFlags.DefaultSampleFlagsPresent)
                DefaultSampleFlags = new SampleFlags(stream.ReadBEUInt32());

            if ((Flags & TrackFragmentFlags.DurationIsEmpty) == TrackFragmentFlags.DurationIsEmpty)
                DurationIsEmpty = true;
        }

        protected override void SaveToStream(Stream stream)
        {
            TrackFragmentFlags newFlags = 0;
            if (BaseDataOffset.HasValue) newFlags |= TrackFragmentFlags.BaseDataOffsetPresent;
            if (SampleDescriptionIndex.HasValue) newFlags |= TrackFragmentFlags.SampleDrescriptionIndexPresent;
            if (DefaultSampleDuration.HasValue) newFlags |= TrackFragmentFlags.DefaultSampleDurationPresent;
            if (DefaultSampleSize.HasValue) newFlags |= TrackFragmentFlags.DefaultSampleSizePresent;
            if (DefaultSampleFlags != null) newFlags |= TrackFragmentFlags.DefaultSampleFlagsPresent;
            if (DurationIsEmpty) newFlags |= TrackFragmentFlags.DurationIsEmpty;

            Flags = newFlags;

            base.SaveToStream(stream);

            stream.WriteBEUInt32(TrackID);
            if (BaseDataOffset.HasValue) stream.WriteBEUInt64(BaseDataOffset.Value);
            if (SampleDescriptionIndex.HasValue) stream.WriteBEUInt32(SampleDescriptionIndex.Value);
            if (DefaultSampleDuration.HasValue) stream.WriteBEUInt32(DefaultSampleDuration.Value);
            if (DefaultSampleSize.HasValue) stream.WriteBEUInt32(DefaultSampleSize.Value);
            if (DefaultSampleFlags!=null) stream.WriteBEUInt32(DefaultSampleFlags._flags);
        }


        private new TrackFragmentFlags Flags
        {
            get
            {
                return (TrackFragmentFlags)_Flags;
            }
            set
            {
                _Flags = (uint)value;
            }
        }

        public uint TrackID { get; set; }
        public ulong? BaseDataOffset { get; set; }
        public uint? SampleDescriptionIndex { get; set; }
        public uint? DefaultSampleDuration { get; set; }
        public uint? DefaultSampleSize { get; set; }
        public SampleFlags DefaultSampleFlags { get; set; }
        bool DurationIsEmpty { get; set; }

        [FlagsAttribute]
        public enum TrackFragmentFlags : int
        {
            BaseDataOffsetPresent = 0x000001,
            SampleDrescriptionIndexPresent = 0x000002,
            DefaultSampleDurationPresent = 0x000008,
            DefaultSampleSizePresent = 0x000010,
            DefaultSampleFlagsPresent = 0x000020,
            DurationIsEmpty = 0x010000,
        }

    }
}
