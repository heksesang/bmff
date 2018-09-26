using System;
using System.Collections.Generic;

namespace MatrixIO
{
    public readonly struct Fraction : IComparable, IComparable<Fraction>, IEquatable<Fraction>
    {
        private readonly int _numerator;
        private readonly int _denominator;
        private readonly bool _isInLowestTerms;

        public bool IsInLowestTerms => _isInLowestTerms;

        public Fraction(int numerator, int denominator)
        {
            if (denominator == 0)
            {
                throw new ArgumentOutOfRangeException("Must not be 0.", nameof(denominator));
            }

            if (denominator < 0)
            {
                if (numerator < 0)
                {
                    _numerator = Math.Abs(numerator);
                    _denominator = Math.Abs(denominator);
                }
                else
                {
                    _numerator = -numerator;
                    _denominator = Math.Abs(denominator);
                }
            }

            _numerator = numerator;
            _denominator = denominator;
            _isInLowestTerms = false;
        }

        public Fraction(int numerator, int denominator, bool asLowestTerms)
            : this(numerator, denominator)
        {
            if (asLowestTerms)
            {
                var gcd = GreatestCommonDivisor(_numerator, _denominator);
                _numerator = _numerator / gcd;
                _denominator = _denominator / gcd;
            }
            _isInLowestTerms = asLowestTerms;
        }

        private Fraction(bool inLowestTerms, int numerator, int denominator)
            : this(numerator, denominator)
        {
            _isInLowestTerms = inLowestTerms;
        }

        public int Numerator => _numerator;

        public int Denominator => _denominator;

        public Fraction GetLowestTerms()
        {
            if (_isInLowestTerms) return this;
            var gcd = GreatestCommonDivisor(_numerator, _denominator);
            return new Fraction(true, _numerator / gcd, _denominator / gcd);
        }

        public Fraction GetReciprocal()
        {
            return new Fraction(_isInLowestTerms, _denominator, _numerator);
        }

        public int GetIntegerPart()
        {
            return Numerator / Denominator;
        }

        public IEnumerable<int> GetFractionalPartEnumerator()
        {
            var remainder = (long)Numerator % Denominator;
            while (remainder > 0)
            {
                var newNumerator = (remainder > Denominator) ? remainder : remainder * 10;
                var digit = newNumerator / Denominator;
                yield return (int)digit;
                remainder = (long)newNumerator % Denominator;
            }
        }

        #region Arithmetic Operators

        public static Fraction operator +(Fraction a, Fraction b)
        {
            var lcm = LeastCommonMultiple(a.Denominator, b.Denominator);
            var numerator = (a.Numerator * lcm) + (b.Numerator * lcm);
            var denominator = (a.Denominator * lcm) + (b.Denominator * lcm);
            return new Fraction(numerator, denominator, true);
        }

        public static Fraction operator -(Fraction a, Fraction b)
        {
            var lcm = LeastCommonMultiple(a.Denominator, b.Denominator);
            var numerator = (a.Numerator * lcm) - (b.Numerator * lcm);
            var denominator = (a.Denominator * lcm) - (b.Denominator * lcm);
            return new Fraction(numerator, denominator, true);
        }

        public static Fraction operator *(Fraction a, Fraction b)
        {
            return new Fraction(a.Numerator * b.Numerator, a.Denominator * b.Denominator);
        }

        public static Fraction operator /(Fraction a, Fraction b)
        {
            return a * b.GetReciprocal();
        }

        #endregion

        #region Conversion Operators

        public static explicit operator float(Fraction a)
        {
            return (float)a.Numerator / a.Denominator;
        }

        public static explicit operator double(Fraction a)
        {
            return (double)a.Numerator / a.Denominator;
        }

        public static explicit operator decimal(Fraction a)
        {
            return (decimal)a.Numerator / a.Denominator;
        }

        public static implicit operator Fraction(int a)
        {
            return new Fraction(true, a, 1);
        }

        #endregion

        #region Equality Operators

        public static bool operator ==(Fraction a, Fraction b)
        {
            return ((long)a.Numerator * b.Denominator) == ((long)a.Denominator * b.Numerator);
        }

        public static bool operator !=(Fraction a, Fraction b)
        {
            return !(a == b);
        }

        public static bool operator >(Fraction a, Fraction b)
        {
            return ((long)a.Numerator * b.Denominator) > ((long)a.Denominator * b.Numerator);
        }

        public static bool operator <(Fraction a, Fraction b)
        {
            return ((long)a.Numerator * b.Denominator) < ((long)a.Denominator * b.Numerator);
        }

        #endregion

        #region IComparable<Fraction> Implementation

        public int CompareTo(Fraction other)
        {
            return (int)(((long)Numerator * other.Denominator) - ((long)Denominator * other.Numerator));
        }

        public int CompareTo(object obj) => throw new NotImplementedException();

        #endregion

        public bool Equals(Fraction other)
        {
            return this == other;
        }

        #region Object overrides

        public override bool Equals(object obj)
        {
            return obj is Fraction other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _numerator.GetHashCode() ^ _denominator.GetHashCode();
        }

        public override string ToString()
        {
            // TODO: Call ToString(string format) instead once it's written.
            return $"{_numerator}/{_denominator}";
        }
        
        #endregion
        
        public static Fraction FromBytes(byte[] buffer, int offset)
        {
            throw new NotImplementedException();
        }

        public static int LeastCommonMultiple(int a, int b)
        {
            return LeastCommonMultiple(a, b, GreatestCommonDivisor(a, b));
        }

        public static int LeastCommonMultiple(int a, int b, int gcd)
        {
            return (a * b) / gcd;
        }

        public static int GreatestCommonDivisor(int dividend, int divisor)
        {
            var remainder = -1;

            while (remainder != 0)
            {
                int quotient = dividend / divisor;
                remainder = dividend % divisor;

                if (remainder != 0)
                {
                    dividend = divisor;
                    divisor = remainder;
                }
            }

            return divisor;
        }
    }
}
