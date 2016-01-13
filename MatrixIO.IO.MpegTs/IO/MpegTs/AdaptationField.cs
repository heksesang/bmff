using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MatrixIO.IO.MpegTs
{
    public class AdaptationField
    {
        public byte Length { get; private set; }
        private byte _flags;
        public bool DiscontinuityIndicator
        {
            get { return (_flags & 0x80) > 0; }
            set { if (value) _flags |= 0x80; else _flags &= 0x7F; }
        }
        public bool RandomAccessIndicator
        {
            get { return (_flags & 0x40) > 0; }
            set { if (value) _flags |= 0x40; else _flags &= 0xBF; }
        }
        public bool ElementaryStreamPriorityIndicator
        {
            get { return (_flags & 0x20) > 0; }
            set { if (value) _flags |= 0x20; else _flags &= 0xDF; }
        }
        private bool HasProgramClockReference
        {
            get { return (_flags & 0x10) > 0; }
            set { if (value) _flags |= 0x10; else _flags &= 0xEF; }
        }
        private bool HasOriginalProgramClockReference
        {
            get { return (_flags & 0x08) > 0; }
            set { if (value) _flags |= 0x08; else _flags &= 0xF7; }
        }
        private bool HasSpliceCountdown
        {
            get { return (_flags & 0x04) > 0; }
            set { if (value) _flags |= 0x04; else _flags &= 0xFB; }
        }
        public bool HasPrivateData
        {
            get { return (_flags & 0x02) > 0; }
            set { if (value) _flags |= 0x02; else _flags &= 0xFD; }
        }
        public bool HasExtension
        {
            get { return (_flags & 0x01) > 0; }
            set { if (value) _flags |= 0x01; else _flags &= 0xFE; }
        }
 
        // Int48 with 33bit base, 6bit pad, 9bit extension
        private byte[] _programClockReference;
        public TimeSpan? ProgramClockReference
        {
            get
            {
                if (_programClockReference == null) return null;
                else return ProgramClockReferenceToTimeSpan(_programClockReference);
            }
            /*
            set
            {
                if (value == null)
                {
                    _programClockReference = null;
                    HasProgramClockReference = false;
                }
                else
                {
                    
                }
            }
            */
        }
        // Int48 with 33bit base, 6bit pad, 9bit extension
        private byte[] _originalProgramClockReference;
        public TimeSpan? OriginalProgramClockReference
        {
            get
            {
                if (_originalProgramClockReference == null) return null;
                else return ProgramClockReferenceToTimeSpan(_originalProgramClockReference);
            }
            /*
            set
            {
                if (value == null)
                {
                    _originalProgramClockReference = null;
                    HasOriginalProgramClockReference = false;
                }
                else
                {

                }
            }
            */
        }

        private sbyte _spliceCountdown;
        public sbyte? SpliceCountdown
        {
            get { return HasSpliceCountdown ? _spliceCountdown : (sbyte?)null; }
            set
            {
                if (value.HasValue) _spliceCountdown = value.Value;
                HasSpliceCountdown = value.HasValue;
            }
        }

        private byte[] _privateData;
        public byte[] PrivateData
        {
            get { return HasPrivateData ? _privateData : null; }
            set { _privateData = value; HasPrivateData = (value != null); }
        }

        private AdaptationFieldExtension _extension;
        public AdaptationFieldExtension Extension
        {
            get { return HasExtension ? _extension : null; }
            set { _extension = value; HasExtension = (value != null); }
        }

        public byte[] Stuffing { get; set; }

        public AdaptationField() { }

        public AdaptationField(byte[] buffer, int offset = 4)
        {
            var position = offset;
            var length = Length = buffer[position++];
            if (length <= 0) return;

            _flags = buffer[position++];

            if (HasProgramClockReference)
            {
                _programClockReference = new byte[6];
                Buffer.BlockCopy(buffer, position, _programClockReference, 0, 6);
                position += 6;
                Debug.WriteLine("Has Program Clock Reference: " + ProgramClockReference);
            }

            if (HasOriginalProgramClockReference)
            {
                Debug.WriteLine("Has Original Program Clock Reference.");
                _originalProgramClockReference = new byte[6];
                Buffer.BlockCopy(buffer, position, _originalProgramClockReference, 0, 6);
                position += 6;
            }

            if (HasSpliceCountdown)
            {
                Debug.WriteLine("Has Splice Countdown.");
                SpliceCountdown = (sbyte) buffer[position++];
            }

            if (HasPrivateData)
            {
                byte privateDataLength = buffer[position++];
                Debug.WriteLine("Has " + privateDataLength + " byte of Private Data.");
                if (position > TsPacket.Length)
                {
                    privateDataLength = 0;
                    Debug.WriteLine("_position exceeds length by " + Math.Abs(buffer.Length - position) + " bytes.");
                    throw new FormatException("Invalid private data length.");
                }
                else if (position + privateDataLength > TsPacket.Length)
                {
                    Debug.WriteLine(" Private data exceeds length by " +
                                    Math.Abs(TsPacket.Length - position - privateDataLength) + " bytes!!!!!!!");
                    privateDataLength = (byte)(TsPacket.Length - position);
                    Debug.WriteLine("Reading " + privateDataLength + " bytes of Private Data.");
                }
                if (privateDataLength > 0)
                {
                    var privateData = new byte[privateDataLength];
                    Buffer.BlockCopy(buffer, position, privateData, 0, privateDataLength);
                    PrivateData = privateData;
                    position += privateDataLength;
                }
            }

            if (HasExtension)
            {
                Debug.WriteLine("Has Extension.");
                Extension = new AdaptationFieldExtension(buffer, position);
                position += Extension.Length;
            }

            int stuffingLength = length + 1 - (position - offset);
            if (stuffingLength > 0)
            {
                Debug.WriteLine("Has " + stuffingLength + " bytes of Adaptation Field stuffing.");
                Stuffing = new byte[stuffingLength];
                Buffer.BlockCopy(buffer, position, Stuffing, 0, stuffingLength);
            }
        }

        private TimeSpan ProgramClockReferenceToTimeSpan(byte[] pcr)
        {
            var longPCR = new long[] {pcr[0], pcr[1], pcr[2], pcr[3], pcr[4], pcr[5]};
            long pcrBase = longPCR[0] << 25 | longPCR[1] << 17 | longPCR[2] << 9 | longPCR[3] << 1 | (longPCR[4] & 0x80) >> 7;
            long pcrExtension = (longPCR[4] & 0x01) << 8 | longPCR[5];

            long pcrBaseTicks = (pcrBase * 1111111) / 10000; // Convert 90khz to Ticks (multiply by 111.1111)
            long pcrExtensionTicks = ((pcrExtension * 10) / 27) / 10; // Convert 27Mhz to Ticks (divide by 2.7)

            long pcrTicks = pcrBaseTicks + pcrExtensionTicks;
            Debug.WriteLine("PCR Ticks: " + pcrTicks);
            return new TimeSpan(pcrTicks);
        }
    }

}
