using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common.Messaging.Parameters;

namespace SCADA.Common.Messaging
{
    public class MessageHeaderBuilder
    {
        public byte[] Build(DNP3CommandParameters param)
        {
            byte[] header = new byte[10];

            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)param.Start)), 0, header, 0, 2);
            header[2] = param.Length;
            header[3] = param.Control;
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)param.Destination)), 0, header, 4, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)param.Source)), 0, header, 6, 2);
            ushort crc = 0;
            for (int i = 0; i < 8; i++)
            {
                CrcCalculator.computeCRC(header[i], ref crc);
            }
            crc = (ushort)(~crc);
            Buffer.BlockCopy(BitConverter.GetBytes((short)crc), 0, header, 8, 2);

            return header;
        }
    }
}
