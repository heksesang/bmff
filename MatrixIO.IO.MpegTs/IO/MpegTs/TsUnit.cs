using System;
using System.Collections.Generic;
using System.Linq;

namespace MatrixIO.IO.MpegTs
{
    public class TsUnit
    {
        private readonly IList<TsPacket> _packets;
        public IList<TsPacket> Packets => _packets;

        private readonly TsUnitPayload _payload;
        public IList<byte> Payload => _payload;

        public TsUnit()
        {
            _packets = new List<TsPacket>();
            _payload = new TsUnitPayload(_packets);
        }

        public TsUnit(int capacity)
        {
            _packets = new List<TsPacket>(capacity);
            _payload = new TsUnitPayload(_packets);
        }

        public TsUnit(IEnumerable<TsPacket> packets)
        {
            _packets = new List<TsPacket>(packets);
            _payload = new TsUnitPayload(_packets);
        }

        private class TsUnitPayload : IList<byte>
        {
            private readonly IList<TsPacket> _packets;

            public TsUnitPayload(IList<TsPacket> packets)
            {
                _packets = packets;
            }

            public int IndexOf(byte item)
            {
                throw new NotSupportedException();
            }

            public void Insert(int index, byte item)
            {
                throw new NotSupportedException();
            }

            public void RemoveAt(int index)
            {
                throw new NotSupportedException();
            }

            public byte this[int index]
            {
                get
                {
                    foreach (var packet in _packets)
                    {
                        if (index < packet.Payload.Length) return packet.Payload[index];
                        index -= packet.Payload.Length;
                    }
                    throw new IndexOutOfRangeException();
                }
                set
                {
                    throw new NotSupportedException();
                }
            }

            public void Add(byte item)
            {
                throw new NotSupportedException();
            }

            public void Clear()
            {
                throw new NotSupportedException();
            }

            public bool Contains(byte item)
            {
                return _packets.Any(packet => packet.Payload.Contains(item));
            }

            public void CopyTo(byte[] array, int arrayIndex)
            {
                var offset = arrayIndex;
                foreach (var packet in _packets)
                {
                    if (packet.Payload != null)
                    {
                        Buffer.BlockCopy(packet.Payload, 0, array, offset, packet.Payload.Length);
                        offset += packet.Payload.Length;
                    }
                }
            }

            public int Count => _packets.Sum(packet => packet.Payload != null ? packet.Payload.Length : 0);

            public bool IsReadOnly => true;

            public bool Remove(byte item)
            {
                throw new NotSupportedException();
            }

            public IEnumerator<byte> GetEnumerator()
            {
                return _packets.SelectMany(packet => packet.Payload).GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
