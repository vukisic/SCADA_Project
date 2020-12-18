using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common.DataModel;
using SCADA.Common.Messaging.Parameters;

namespace SCADA.Common.Messaging.Messages
{
    public class ReadAnalogOutput : DNP3Function
    {
        private MessageHeaderBuilder headerBuilder;
        public ReadAnalogOutput(DNP3ApplicationObjectParameters commandParameters, string commandOwner) : base(commandParameters, commandOwner) 
        {
            headerBuilder = new MessageHeaderBuilder();
        }

        public override byte[] PackRequest()
        {
            byte[] request = new byte[20];
            CommandParameters.Length = 0x0d; //20 - 2*(2 CRC) - 2 Start - 1 len
            Buffer.BlockCopy(headerBuilder.Build(CommandParameters), 0, request, 0, 10);
            request[10] = CommandParameters.TransportHeader;
            request[11] = CommandParameters.AplicationControl;
            request[12] = CommandParameters.FunctionCode;
            request[13] = BitConverter.GetBytes((short)CommandParameters.TypeField)[1];
            request[14] = BitConverter.GetBytes((short)CommandParameters.TypeField)[0];
            request[15] = CommandParameters.Qualifier;
            request[16] = (byte)CommandParameters.Range;
            request[17] = (byte)CommandParameters.Range;
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
