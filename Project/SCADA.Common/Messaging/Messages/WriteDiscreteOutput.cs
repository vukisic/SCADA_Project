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
    public class WriteDiscreteOutput : DNP3Function
    {
        private MessageHeaderBuilder headerBuilder;
        public WriteDiscreteOutput(DNP3CommandParameters commandParameters) : base(commandParameters)
        {
            headerBuilder = new MessageHeaderBuilder();
        }
        public override byte[] PackRequest()
        {
            byte[] request = new byte[35];

            DNP3WriteCommandParameters commandParam = (DNP3WriteCommandParameters)CommandParameters;

            CommandParameters.Length = 0x1a;
            Buffer.BlockCopy(headerBuilder.Build(CommandParameters), 0, request, 0, 10);
            request[10] = commandParam.TransportControl;
            request[11] = commandParam.AplicationControl;
            request[12] = commandParam.FunctionCode;
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)commandParam.ObjectTypeField)), 0, request, 13, 2);
            request[15] = commandParam.Qualifier;
            Buffer.BlockCopy(BitConverter.GetBytes(Convert.ToInt16(commandParam.RangeField)), 0, request, 16, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(Convert.ToInt16(commandParam.Prefix)), 0, request, 18, 2);

            if (commandParam.Value == 1)
            {
                request[20] = 0x41;
            }
            else
            {
                request[20] = 0x81;
            }

            request[21] = 0x01;
            request[22] = 0x00;
            request[23] = 0x00;
            request[24] = 0x00;
            request[25] = 0x00;

            ushort crc2 = 0;
            for (int i = 10; i < 26; i++)
            {
                CrcCalculator.computeCRC(request[i], ref crc2);
            }
            crc2 = (ushort)(~crc2);
            Buffer.BlockCopy(BitConverter.GetBytes(crc2), 0, request, 26, 2);

            request[28] = 0x00;
            request[29] = 0x00;
            request[30] = 0x00;
            request[31] = 0x00;
            request[32] = 0x00;

            ushort crc1 = 0;
            for (int i = 28; i < 33; i++)
            {
                CrcCalculator.computeCRC(request[i], ref crc1);
            }
            crc1 = (ushort)(~crc1);

            Buffer.BlockCopy(BitConverter.GetBytes(crc1), 0, request, 33, 2);

            return request;
        }

        public override Dictionary<Tuple<RegisterType, ushort>, ushort> PareseResponse(byte[] response)
        {
            throw new NotImplementedException();
        }
    }
}
