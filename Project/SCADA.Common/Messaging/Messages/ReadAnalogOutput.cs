using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common.DataModel;
using SCADA.Common.Messaging.Parameters;

namespace SCADA.Common.Messaging.Messages
{
    public class ReadAnalogOutput : DNP3Function
    {
        private MessageHeaderBuilder headerBuilder;
        public ReadAnalogOutput(DNP3ApplicationObjectParameters commandParameters) : base(commandParameters) 
        {
            headerBuilder = new MessageHeaderBuilder();
        }

        public override byte[] PackRequest()
        {
            byte[] request = new byte[20];

            CommandParameters.Length = 0x0d; //20 - 2*(2 CRC) - 2 Start - 1 len
            Buffer.BlockCopy(headerBuilder.Build(CommandParameters), 0, request, 0, 10);
            request[10] = CommandParameters.TransportControl;
            request[11] = CommandParameters.AplicationControl;
            request[12] = CommandParameters.FunctionCode;
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)CommandParameters.ObjectTypeField)), 0, request, 13, 2);
            request[15] = CommandParameters.Qualifier;
            request[16] = (byte)CommandParameters.RangeField;
            request[17] = (byte)CommandParameters.RangeField;
            ushort crc = 0;
            for (int i = 10; i < 18; i++)
            {
                CrcCalculator.computeCRC(request[i], ref crc);
            }
            crc = (ushort)(~crc);
            Buffer.BlockCopy(BitConverter.GetBytes(crc), 0, request, 18, 2);

            return request;
        }
    }
}
