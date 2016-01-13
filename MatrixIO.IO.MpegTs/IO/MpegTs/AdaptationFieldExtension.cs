using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MatrixIO.IO.MpegTs
{
    public class AdaptationFieldExtension
    {
        public byte Length { get; private set; }
        private byte _flags;

        public bool HasLegalTimeWindow
        {
            get { return (_flags & 0x80) > 0; }
            set { if (value) _flags |= 0x80; else _flags &= 0x7F; }
        }
        public bool HasPiecewiseRate
        {
            get { return (_flags & 0x40) > 0; }
            set { if (value) _flags |= 0x40; else _flags &= 0xBF; }
        }
        public bool HasSeamlessSplice
        {
            get { return (_flags & 0x20) > 0; }
            set { if (value) _flags |= 0x20; else _flags &= 0xDF; }
        }

        public byte[] Stuffing { get; set; }

        public AdaptationFieldExtension() { }
        public AdaptationFieldExtension(byte[] buffer, int offset = 0)
        {
            int position = offset;

            int length = Length = buffer[position++];
            if (length > 0)
            {
                _flags = buffer[position++];

                if(HasLegalTimeWindow)
                {
                    Debug.WriteLine("Has Legal Time Window");
                    // TODO: Support Legal Time Window
                    offset += 2;
                }

                if(HasPiecewiseRate)
                {
                    Debug.WriteLine("Has Piecewise Rate");
                    // TODO: Support Piecewise Rate
                    offset += 3;
                }

                if(HasSeamlessSplice)
                {
                    Debug.WriteLine("Has Seamless Splice");
                    // TODO: Support Piecewise Splice
                    offset += 5;
                }

                int stuffingLength = length + 1 - (position - offset);
                if (stuffingLength > 0)
                {
                    Debug.WriteLine("Has " + stuffingLength + " bytes of Adaptation Field Extension Stuffing.");
                    Stuffing = new byte[stuffingLength];
                    Buffer.BlockCopy(buffer, position, Stuffing, 0, stuffingLength);
                }
            }
        }
    }
}
