using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Track Header Box ('tkhd')
    /// </summary>
    [Box("tkhd", "Track Header Box")]
    public class TrackHeaderBox : FullBox
    {
        public TrackHeaderBox() : base() { }
        public TrackHeaderBox(Stream stream) : base(stream) { }

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() +
                (ulong)(Version == 1 ? 8 + 8 + 4 + 4 + 8 : 4 + 4 + 4 + 4 + 4) + (2 * 4) + 2 + 2 + 2 + 2 + (9 * 4) + 4 + 4;
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            if (Version == 1)
            {
                CreationTime = MovieHeaderBox.Convert1904Time(stream.ReadBEUInt64());
                ModificationTime = MovieHeaderBox.Convert1904Time(stream.ReadBEUInt64());
                TrackID = stream.ReadBEUInt32();
                _Reserved1 = stream.ReadBEUInt32();
                Duration = stream.ReadBEUInt64();
            }
            else // if (Version == 0)
            {
                CreationTime = MovieHeaderBox.Convert1904Time(stream.ReadBEUInt32());
                ModificationTime = MovieHeaderBox.Convert1904Time(stream.ReadBEUInt32());
                TrackID = stream.ReadBEUInt32();
                _Reserved1 = stream.ReadBEUInt32();
                Duration = stream.ReadBEUInt32();
            }
            for (int i = 0; i < 2; i++) _Reserved2[0] = stream.ReadBEUInt32();
            Layer = stream.ReadBEInt16();
            AlternateGroup = stream.ReadBEInt16();
            Volume = stream.ReadBEInt16();
            _Reserved3 = stream.ReadBEUInt16();
            for (int i = 0; i < 9; i++) _Matrix[i] = stream.ReadBEInt32();
            Width = stream.ReadBEUInt32();
            Height = stream.ReadBEUInt32();
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            if (Version == 1)
            {
                stream.WriteBEUInt64(MovieHeaderBox.Convert1904Time(CreationTime));
                stream.WriteBEUInt64(MovieHeaderBox.Convert1904Time(ModificationTime));
                stream.WriteBEUInt32(TrackID);
                stream.WriteBEUInt32(_Reserved1);
                stream.WriteBEUInt64(Duration);
            }
            else // if (Version == 0)
            {
                stream.WriteBEUInt32((uint)MovieHeaderBox.Convert1904Time(CreationTime));
                stream.WriteBEUInt32((uint)MovieHeaderBox.Convert1904Time(ModificationTime));
                stream.WriteBEUInt32(TrackID);
                stream.WriteBEUInt32(_Reserved1);
                stream.WriteBEUInt32((uint)Duration);
            }
            for(int i=0; i<2; i++) stream.WriteBEUInt32(_Reserved2[i]);
            stream.WriteBEInt16(Layer);
            stream.WriteBEInt16(AlternateGroup);
            stream.WriteBEInt16(Volume);
            stream.WriteBEUInt16(_Reserved3);
            for (int i = 0; i < 9; i++) stream.WriteBEInt32(_Matrix[i]);
            stream.WriteBEUInt32(Width);
            stream.WriteBEUInt32(Height);
        }

        public DateTime CreationTime { get; set; }
        public DateTime ModificationTime { get; set; }
        public uint TrackID { get; set; }
        private uint _Reserved1;
        public ulong Duration { get; set; }

        private uint[] _Reserved2 = new uint[2];
        public short Layer { get; set; }
        public short AlternateGroup { get; set; }
        public short Volume { get; set; }
        private ushort _Reserved3;
        private int[] _Matrix = new int[9] { 0x00010000, 0, 0, 0, 0x00010000, 0, 0, 0, 0x40000000 }; // Unity Matrix
        public int[] Matrix { get { return _Matrix; } }

        public uint Width { get; set; }
        public uint Height { get; set; }
    }
}
