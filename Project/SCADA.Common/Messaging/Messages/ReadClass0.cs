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
    public class ReadClass0 : DNP3Function
    {
        private MessageHeaderBuilder headerBuilder;

        public ReadClass0(DNP3CommandParameters commandParameters) : base(commandParameters)
        {
            headerBuilder = new MessageHeaderBuilder();
        }
        public override byte[] PackRequest()
        {
            byte[] request = new byte[18];

            DNP3ReadCommandParameters commandParam = (DNP3ReadCommandParameters)CommandParameters;

            CommandParameters.Length = 0x0b;  
            Buffer.BlockCopy(headerBuilder.Build(CommandParameters), 0, request, 0, 10);
            request[10] = commandParam.TransportControl;
            request[11] = commandParam.AplicationControl;
            request[12] = commandParam.FunctionCode;
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)commandParam.ObjectTypeField)), 0, request, 13, 2);
            request[15] = commandParam.Qualifier;
            ushort crc = 0;
            for (int i = 10; i < 16; i++)
            {
                CrcCalculator.computeCRC(request[i], ref crc);
            }
            crc = (ushort)(~crc);
            Buffer.BlockCopy(BitConverter.GetBytes(crc), 0, request, 16, 2);

            return request;
        }

        public override Dictionary<Tuple<RegisterType, ushort>, BasePoint> PareseResponse(byte[] response)
        {
            if (!CrcCalculator.CheckCRC(response))
                return null;

            byte[] dataObjects = MessagesHelper.GetResponseDataObjects(response);

            Dictionary<Tuple<RegisterType, ushort>, BasePoint> retVal = new Dictionary<Tuple<RegisterType, ushort>, BasePoint>();
/*
            ushort typeField = (ushort)IPAddress.NetworkToHostOrder((short)BitConverter.ToUInt16(response, 0));
            int startIndex;
            int stopIndex;
            if(typeField == (ushort)TypeField.BINARY_INPUT_PACKED_FORMAT)
            {
                startIndex = dataObjects[3];
                stopIndex = dataObjects[4];
                int byteCount = (stopIndex - startIndex) % 8 == 0 ? (stopIndex - startIndex) / 8 : (stopIndex - startIndex) / 8 + 1;
                for(int i = 0; i < byteCount; i++)
                {
                    byte currentByte = response[i];
                    for(int j = 0; j < 8; j++)
                    {

                    }
                }
            }*/
            return retVal;
        }
    }
}
