using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Media Header Box ("mdhd")
    /// </summary>
    [Box("mdhd", "Media Header Box")]
    public class MediaHeaderBox : FullBox
    {
        public MediaHeaderBox() : base() { }
        public MediaHeaderBox(Stream stream) : base(stream) { }

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + (ulong)(Version == 1 ? 8 + 8 + 4 + 8 : 4 + 4 + 4 + 4) + 2 + 2;
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            if (Version == 1)
            {
                CreationTime = MovieHeaderBox.Convert1904Time(stream.ReadBEUInt64());
                ModificationTime = MovieHeaderBox.Convert1904Time(stream.ReadBEUInt64());
                TimeScale = stream.ReadBEUInt32();
                Duration = stream.ReadBEUInt64();
            }
            else // if (Version == 0)
            {
                CreationTime = MovieHeaderBox.Convert1904Time(stream.ReadBEUInt32());
                ModificationTime = MovieHeaderBox.Convert1904Time(stream.ReadBEUInt32());
                TimeScale = stream.ReadBEUInt32();
                Duration = stream.ReadBEUInt32();
            }

            Language = ConvertThreeLetterLanguageCode(stream.ReadBEUInt16());
            Predefined = stream.ReadBEUInt16();
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            if (Version == 1)
            {
                stream.WriteBEUInt64(MovieHeaderBox.Convert1904Time(CreationTime));
                stream.WriteBEUInt64(MovieHeaderBox.Convert1904Time(ModificationTime));
                stream.WriteBEUInt32(TimeScale);
                stream.WriteBEUInt64(Duration);
            }
            else // if (Version == 0)
            {
                stream.WriteBEUInt32((uint)MovieHeaderBox.Convert1904Time(CreationTime));
                stream.WriteBEUInt32((uint)MovieHeaderBox.Convert1904Time(ModificationTime));
                stream.WriteBEUInt32(TimeScale);
                stream.WriteBEUInt32((uint)Duration);
            }

            stream.WriteBEUInt16(ConvertThreeLetterLanguageCode(Language));
            stream.WriteBEUInt16(Predefined);
        }

        public DateTime CreationTime { get; set; }
        public DateTime ModificationTime { get; set; }
        public uint TimeScale { get; set; }
        public ulong Duration { get; set; }

        // TODO: Validate this on set.
        public string Language { get; set; }
        public ushort Predefined { get; set; }

        private const ushort CHARBASE = 0x0060;
        private const ushort CHARMASK1 = 0x7C00;
        private const ushort CHARMASK2 = 0x03E0;
        private const ushort CHARMASK3 = 0x001F;

        internal ushort ConvertThreeLetterLanguageCode(string language)
        {
            byte[] langBytes = Encoding.UTF8.GetBytes(language);
            if (langBytes.Length != 3) throw new ArgumentOutOfRangeException();
            return (ushort)((((langBytes[0]-CHARBASE) << 10) & CHARMASK1) | (((langBytes[1]-CHARBASE) << 5) & CHARMASK2) | (((langBytes[2]-CHARBASE) & CHARMASK3)));
        }
        internal string ConvertThreeLetterLanguageCode(ushort language)
        {
            byte[] langBytes = new byte[3];
            langBytes[0] = (byte)(((language & CHARMASK1) >> 10) + CHARBASE);
            langBytes[1] = (byte)(((language & CHARMASK2) >> 5) + CHARBASE);
            langBytes[2] = (byte)((language & CHARMASK3) + CHARBASE);
            return Encoding.UTF8.GetString(langBytes, 0, langBytes.Length);
        }

    }
}
