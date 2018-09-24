using System.Collections.Generic;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Edit List Box ("elst")
    /// </summary>
    [Box("elst", "Edit List Box")]
    public sealed class EditListBox : FullBox, ITableBox<EditListBox.EditListEntry>
    {
        public EditListBox() 
            : base() { }

        public EditListBox(Stream stream) 
            : base(stream) { }

        public EditListEntry[] Entries { get; set; }

        public int EntryCount => Entries.Length;

        internal override ulong CalculateSize()
        {
            ulong entryLength = (ulong)(Version == 1 ? 8 + 8 : 4 + 4) + 2 + 2;
            return base.CalculateSize() + 4 + (entryLength * (ulong)Entries.Length);
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            uint entryCount = stream.ReadBEUInt32();

            this.Entries = new EditListEntry[entryCount];

            for (uint i = 0; i < entryCount; i++)
            {
                ulong segmentDuration;
                long mediaTime;
                short mediaRateInteger;
                short mediaRateFraction;

                if (Version == 1)
                {
                    segmentDuration = stream.ReadBEUInt64();
                    mediaTime = stream.ReadBEInt64();
                }
                else
                {
                    segmentDuration = stream.ReadBEUInt32();
                    mediaTime = stream.ReadBEInt32();
                }

                mediaRateInteger = stream.ReadBEInt16();
                mediaRateFraction = stream.ReadBEInt16();

                Entries[i] = new EditListEntry(segmentDuration, mediaTime, mediaRateInteger, mediaRateFraction);
            }
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            stream.WriteBEInt32(Entries.Length);

            for (int i = 0; i < Entries.Length; i++)
            {
                ref EditListEntry entry = ref Entries[i];

                if (Version == 1)
                {
                    stream.WriteBEUInt64(entry.SegmentDuration);
                    stream.WriteBEInt64(entry.MediaTime);
                }
                else
                {
                    stream.WriteBEUInt32((uint)entry.SegmentDuration);
                    stream.WriteBEInt32((int)entry.MediaTime);
                }

                stream.WriteBEInt16(entry.MediaRateInteger);
                stream.WriteBEInt16(entry.MediaRateFraction);
            }
        }
        
        public readonly struct EditListEntry
        {
            public EditListEntry(
                ulong segmentDuration,
                long mediaTime,
                short mediaRateInteger,
                short mediaRateFraction)
            {
                SegmentDuration = segmentDuration;
                MediaTime = mediaTime;
                MediaRateInteger = mediaRateInteger;
                MediaRateFraction = mediaRateFraction;
            }

            public ulong SegmentDuration { get; }

            public long MediaTime { get; }

            public short MediaRateInteger { get; }

            public short MediaRateFraction { get; }
        }
    }
}