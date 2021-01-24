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
    public class WriteAnalogOutput : DNP3Function
    {
        private MessageHeaderBuilder headerBuilder;
        public WriteAnalogOutput(DNP3CommandParameters commandParameters) : base(commandParameters)
        {
            headerBuilder = new MessageHeaderBuilder();
        }
        public override byte[] PackRequest()
        {
            byte[] request = new byte[25];

            DNP3WriteCommandParameters commandParam = (DNP3WriteCommandParameters)CommandParameters;

            CommandParameters.Length = 18;
            Buffer.BlockCopy(headerBuilder.Build(CommandParameters), 0, request, 0, 10);
            request[10] = commandParam.TransportControl;
            request[11] = commandParam.AplicationControl;
            request[12] = commandParam.FunctionCode;
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)commandParam.ObjectTypeField)), 0, request, 13, 2);
            request[15] = commandParam.Qualifier;
            Buffer.BlockCopy(BitConverter.GetBytes(Convert.ToUInt16(commandParam.RangeField)), 0, request, 16, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(Convert.ToUInt16(commandParam.Prefix)), 0, request, 18, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(Convert.ToUInt16(commandParam.Value)), 0, request, 20, 2);
            request[22] = 0x00;

            ushort crc1 = 0;
            for (int i = 10; i < 23; i++)
            {
                CrcCalculator.computeCRC(request[i], ref crc1);
            }
            crc1 = (ushort)(~crc1);

            Buffer.BlockCopy(BitConverter.GetBytes(crc1), 0, request, 23, 2);

            return request;
        }

        public override Dictionary<Tuple<RegisterType, ushort>, ushort> PareseResponse(byte[] response)
        {
            throw new NotImplementedException();
        }
    }
}
