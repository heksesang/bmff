using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MatrixIO.IO.MpegTs.Streams
{
    public class PacketizedElementalStream : TsStream<PesPacket>
    {
        protected override PesPacket ProcessUnit(TsUnit unit)
        {
            // TODO: We should think about passing the unit payload in as an IList for speed.  It complicates copying chunks of data, however.
            var unitPayload = new byte[unit.Payload.Count];
            unit.Payload.CopyTo(unitPayload, 0);

            var pesPacket = new PesPacket(unitPayload);
            Debug.WriteLine("PES Identifier is " + pesPacket.StreamIdentifier);
            return pesPacket;
        }
    }
}
