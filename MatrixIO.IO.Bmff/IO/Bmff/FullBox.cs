using System;
using System.Collections;
using System.IO;

namespace MatrixIO.IO.Bmff
{
    public abstract class FullBox : Box
    {
        protected uint _flags;
        
        protected FullBox() : this(0) { }

        protected FullBox(byte version, uint flags = 0) 
            : base() 
        {
            if ((flags & 0xFF000000) > 0)
            {
                throw new FormatException("Box flags must be 24 bits.");
            }

            Version = version;
            _flags = flags;
        }
        
        protected FullBox(Stream stream)
            : base(stream) { }

        public byte Version { get; set; }

        public BitArray Flags
        {
            get
            {
                byte[] flagBytes32 = BitConverter.GetBytes(_flags.HostToNetworkOrder());
                byte[] flagBytes24 = new byte[3];
                Buffer.BlockCopy(flagBytes32, 1, flagBytes24, 0, 3);
                return new BitArray(flagBytes24);
            }
        }

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + 4;
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            Version = stream.ReadOneByte();
            _flags = stream.ReadBEUInt24();
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            stream.WriteOneByte(Version);
            stream.WriteBEUInt24(_flags);
        }
    }
}