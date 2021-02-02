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

        public override Dictionary<Tuple<RegisterType, int>, BasePoint> PareseResponse(byte[] response)
        {
            if (!CrcCalculator.CheckCRC(response))
                return null;

            byte[] dataObjects = response.Skip(15).ToArray();
            var index = (ushort)BitConverter.ToUInt16(dataObjects.Skip(5).Take(2).ToArray(),0);
            var value = BitConverter.ToUInt16(dataObjects.Skip(7).Take(2).ToArray(), 0);

            Dictionary<Tuple<RegisterType, int>, BasePoint> retVal = new Dictionary<Tuple<RegisterType, int>, BasePoint>();
            AnalogPoint point = new AnalogPoint();
            point.Index = index;
            point.Value = value;
            point.RegisterType = RegisterType.ANALOG_OUTPUT;
            retVal.Add(new Tuple<RegisterType, int>(RegisterType.ANALOG_OUTPUT, point.Index), point);

            return retVal;
        }
    }
}
