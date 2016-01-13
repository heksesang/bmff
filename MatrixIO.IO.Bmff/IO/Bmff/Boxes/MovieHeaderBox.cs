using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Movie Header Box ("mvhd")
    /// </summary>
    [Box("mvhd", "Movie Header Box")]
    public class MovieHeaderBox : FullBox
    {
        public MovieHeaderBox() : base() 
        {
            _Rate = 0x00010000; // 1.0 normal rate
            _Volume = 0x0100; // 1.0 full volume
            Matrix = new int[] { 0x00010000, 0, 0, 0, 0x00010000, 0, 0, 0, 0x40000000 }; // Unity Matrix
        }
        public MovieHeaderBox(Stream stream) : base(stream) { }

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + 
                (ulong)(Version == 1 ? 8 + 8 + 4 + 8 : 4 + 4 + 4 + 4) + 4 + 2 + 2 + (2 * 4) + (9 * 4) + (6 * 4) + 4;

        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            if (Version == 1)
            {
                _CreationTime = stream.ReadBEUInt64();
                _ModificationTime = stream.ReadBEUInt64();
                TimeScale = stream.ReadBEUInt32();
                Duration = stream.ReadBEUInt64();
            }
            else // if(Version == 0)
            {
                _CreationTime = stream.ReadBEUInt32();
                _ModificationTime = stream.ReadBEUInt32();
                TimeScale = stream.ReadBEUInt32();
                Duration = stream.ReadBEUInt32();
            }
            _Rate = stream.ReadBEInt32();
            _Volume = stream.ReadBEInt16();
            Reserved = stream.ReadBytes(2 + (2 * 4));
            for (int i = 0; i < 9; i++) Matrix[i] = stream.ReadBEInt32();
            PreDefined = stream.ReadBytes(6 * 4);
            NextTrackID = stream.ReadBEUInt32();
        }

        protected override void SaveToStream(Stream stream)
        {
            if (Version==0 && 
                (_CreationTime > uint.MaxValue || 
                _ModificationTime > uint.MaxValue || 
                Duration > uint.MaxValue)) Version = 1;

            base.SaveToStream(stream);

            if (Version == 1)
            {
                stream.WriteBEUInt64(_CreationTime);
                stream.WriteBEUInt64(_ModificationTime);
                stream.WriteBEUInt32(TimeScale);
                stream.WriteBEUInt64(Duration);
            }
            else // if(Version == 0)
            {
                stream.WriteBEUInt32(checked((uint)_CreationTime));
                stream.WriteBEUInt32(checked((uint)_ModificationTime));
                stream.WriteBEUInt32(TimeScale);
                stream.WriteBEUInt32(checked((uint)Duration));
            }
            stream.WriteBEInt32(_Rate);
            stream.WriteBEInt16(_Volume);
            stream.WriteBytes(Reserved);
            for (int i = 0; i < 9; i++) stream.WriteBEInt32(Matrix[i]);
            stream.WriteBytes(PreDefined);
            stream.WriteBEUInt32(NextTrackID);
        }



        private ulong _CreationTime;
        public DateTime CreationTime
        {
            get
            {
                return Convert1904Time(_CreationTime);
            }
            set
            {
                _CreationTime = Convert1904Time(value);
            }
        }

        private ulong _ModificationTime;
        public DateTime ModificationTime
        {
            get
            {
                return Convert1904Time(_ModificationTime);
            }
            set
            {
                _ModificationTime = Convert1904Time(value);
            }
        }

        public uint TimeScale { get; set; }

        public ulong Duration { get; set; }

        private int _Rate;
        public double Rate
        {
            get
            {
                return (double)_Rate / ((int)ushort.MaxValue + 1);
            }
            set
            {
                _Rate = checked((int)Math.Round(value * ((int)short.MaxValue + 1)));
            }
        }

        private short _Volume;
        public double Volume
        {
            get
            {
                return (double)_Volume / ((int)byte.MaxValue + 1);
            }
            set
            {
                _Volume = checked((short)Math.Round(value * ((int)byte.MaxValue + 1)));
            }
        }

        public byte[] Reserved { get; private set; }

        public int[] Matrix { get; private set; }

        public byte[] PreDefined { get; private set; }

        public uint NextTrackID { get; set; }


        internal static readonly DateTime _1904BaseTime = new DateTime(1904, 1, 1);
        internal static DateTime Convert1904Time(ulong secondsSince1904)
        {
            return _1904BaseTime + TimeSpan.FromSeconds(checked((double)secondsSince1904));
        }
        internal static ulong Convert1904Time(DateTime time)
        {
            return checked((ulong)Math.Round((time - _1904BaseTime).TotalSeconds));
        }
    }
}
