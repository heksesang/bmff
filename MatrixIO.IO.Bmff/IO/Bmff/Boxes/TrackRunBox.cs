using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Track Run Box ("trun")
    /// </summary>
    [Box("trun", "Track Run Box")]
    public class TrackRunBox : FullBox, ITableBox<TrackRunBox.TrackRunEntry>
    {
        public TrackRunBox() : base() { }
        public TrackRunBox(Stream stream) : base(stream) { }

        internal override ulong CalculateSize()
        {
            ulong calculatedSize = base.CalculateSize() + 4;

            if ((Flags & TrackRunFlags.DataOffsetPresent) == TrackRunFlags.DataOffsetPresent)
                calculatedSize += 4;

            if ((Flags & TrackRunFlags.FirstSampleFlagsPresent) == TrackRunFlags.FirstSampleFlagsPresent)
                calculatedSize += 4;

            ulong entryLength = 0;
            if ((Flags & TrackRunFlags.SampleDurationPresent) == TrackRunFlags.SampleDurationPresent)
                entryLength += 4;
            if ((Flags & TrackRunFlags.SampleSizePresent) == TrackRunFlags.SampleSizePresent)
                entryLength += 4;
            if ((Flags & TrackRunFlags.SampleFlagsPresent) == TrackRunFlags.SampleFlagsPresent)
                entryLength += 4;
            if ((Flags & TrackRunFlags.SampleCompositionTimeOffsetsPresent) == TrackRunFlags.SampleCompositionTimeOffsetsPresent)
                entryLength += 4;

            return calculatedSize + (entryLength * (ulong)_Entries.Count);
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            uint sampleCount = stream.ReadBEUInt32();

            if ((Flags & TrackRunFlags.DataOffsetPresent) == TrackRunFlags.DataOffsetPresent)
                DataOffset = stream.ReadBEInt32();
            
            if ((Flags & TrackRunFlags.FirstSampleFlagsPresent) == TrackRunFlags.FirstSampleFlagsPresent)
                FirstSampleFlags = new SampleFlags(stream.ReadBEUInt32());

            for (uint i = 0; i < sampleCount; i++)
            {
                TrackRunEntry entry = new TrackRunEntry();
                if ((Flags & TrackRunFlags.SampleDurationPresent) == TrackRunFlags.SampleDurationPresent)
                    entry.SampleDuration = stream.ReadBEUInt32();
                if ((Flags & TrackRunFlags.SampleSizePresent) == TrackRunFlags.SampleSizePresent)
                    entry.SampleSize = stream.ReadBEUInt32();
                if ((Flags & TrackRunFlags.SampleFlagsPresent) == TrackRunFlags.SampleFlagsPresent)
                    entry.SampleFlags = new SampleFlags(stream.ReadBEUInt32());
                if ((Flags & TrackRunFlags.SampleCompositionTimeOffsetsPresent) == TrackRunFlags.SampleCompositionTimeOffsetsPresent)
                    entry.SampleCompositionTimeOffset = stream.ReadBEUInt32();

                _Entries.Add(entry);
            }
        }

        protected override void SaveToStream(Stream stream)
        {
            if (DataOffset.HasValue) Flags |= TrackRunFlags.DataOffsetPresent;
            if (FirstSampleFlags != null) Flags |= TrackRunFlags.FirstSampleFlagsPresent;
            foreach (var entry in _Entries)
            {
                // TODO: There must be a better way... probably involves changing TrackRunEntry
                if (entry.SampleDuration.HasValue) Flags |= TrackRunFlags.SampleDurationPresent;
                if (entry.SampleSize.HasValue) Flags |= TrackRunFlags.SampleSizePresent;
                if (entry.SampleFlags != null) Flags |= TrackRunFlags.SampleFlagsPresent;
                if (entry.SampleCompositionTimeOffset.HasValue) Flags |= TrackRunFlags.SampleCompositionTimeOffsetsPresent;
            }
            base.SaveToStream(stream);

            stream.WriteBEUInt32((uint)_Entries.Count);

            if (DataOffset.HasValue)
                stream.WriteBEInt32(DataOffset.Value);

            if (FirstSampleFlags != null)
                stream.WriteBEUInt32(FirstSampleFlags._flags);

            foreach (TrackRunEntry entry in _Entries)
            {
                if ((Flags & TrackRunFlags.SampleDurationPresent) == TrackRunFlags.SampleDurationPresent)
                    stream.WriteBEUInt32(entry.SampleDuration.HasValue ? entry.SampleDuration.Value : 0);
                if ((Flags & TrackRunFlags.SampleSizePresent) == TrackRunFlags.SampleSizePresent)
                    stream.WriteBEUInt32(entry.SampleSize.HasValue ? entry.SampleSize.Value : 0);
                if ((Flags & TrackRunFlags.SampleFlagsPresent) == TrackRunFlags.SampleFlagsPresent)
                    stream.WriteBEUInt32(entry.SampleFlags != null ? entry.SampleFlags._flags : 0);
                if ((Flags & TrackRunFlags.SampleCompositionTimeOffsetsPresent) == TrackRunFlags.SampleCompositionTimeOffsetsPresent)
                    stream.WriteBEUInt32(entry.SampleCompositionTimeOffset.HasValue ? entry.SampleCompositionTimeOffset.Value : 0);
            }
        }

        private new TrackRunFlags Flags
        {
            get
            {
                return (TrackRunFlags)_Flags;
            }
            set
            {
                _Flags = (uint)value;
            }
        }

        public int? DataOffset { get; set; }
        public SampleFlags FirstSampleFlags { get; set; }
        
        private IList<TrackRunEntry> _Entries = Portability.CreateList<TrackRunEntry>();
        public IList<TrackRunEntry> Entries { get { return _Entries; } }

        [FlagsAttribute]
        public enum TrackRunFlags : uint
        {
            DataOffsetPresent = 0x000001,
            FirstSampleFlagsPresent = 0x000004,
            SampleDurationPresent = 0x000100,
            SampleSizePresent = 0x000200,
            SampleFlagsPresent = 0x000400,
            SampleCompositionTimeOffsetsPresent = 0x000800,
        }

        public class TrackRunEntry
        {
            public TrackRunEntry() { }

            public uint? SampleDuration { get; set; }
            public uint? SampleSize { get; set; }
            public SampleFlags SampleFlags { get; set; }
            public uint? SampleCompositionTimeOffset { get; set; }
        }
    }
}
