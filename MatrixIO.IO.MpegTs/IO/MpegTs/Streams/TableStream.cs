using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using MatrixIO.IO.MpegTs.Tables;

namespace MatrixIO.IO.MpegTs.Streams
{
    public class TableStream : TsStream<TsTable>
    {
        protected override TsTable ProcessUnit(TsUnit unit)
        {
            // TODO: We should think about passing the unit payload in as an IList for speed.  It complicates copying chunks of data, however.
            var unitPayload = new byte[unit.Payload.Count];
            unit.Payload.CopyTo(unitPayload, 0);

            var pointer = (int)unitPayload[0] + 1;

            var tid = (TableIdentifier)unitPayload[pointer];
            Debug.WriteLine("Received " + tid + " Table");
            switch (tid)
            {
                case TableIdentifier.ProgramAssociation:
                    return new ProgramAssociationTable(unitPayload, pointer, unitPayload.Length - pointer);
                case TableIdentifier.ProgramMap:
                    return new ProgramMapTable(unitPayload, pointer, unitPayload.Length - pointer);
                case TableIdentifier.Description:
                    return new DescriptionTable(unitPayload, pointer, unitPayload.Length - pointer);
                default:
                    Debug.WriteLine("Unsupported Table: " + tid);
                    return null;
            }
        }
    }
}
