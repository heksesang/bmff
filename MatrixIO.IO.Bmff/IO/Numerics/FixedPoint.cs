using System;

namespace MatrixIO.IO.Numerics
{
    /// <summary>
    /// A 16.16 Fixed Point Number
    /// </summary>
    public readonly struct FixedPoint_16_16
    {
        private readonly uint value;

        public FixedPoint_16_16(uint value)
        {
            this.value = value;
        }
        
        public uint Value => value;

        public double ToDouble()
        {
            return (double)value / 65536;
        }

        public static FixedPoint_16_16 FromDouble(double value)
        {
            return new FixedPoint_16_16((uint)Math.Round(value * 65536, 0));
        }

        public static implicit operator FixedPoint_16_16(double value)
        {
            return FromDouble(value);
        }

        public static implicit operator FixedPoint_16_16(uint value)
        {
            return new FixedPoint_16_16(value);
        }
    }
}