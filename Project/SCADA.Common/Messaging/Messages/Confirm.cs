using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common.Messaging.Parameters;

namespace SCADA.Common.Messaging.Messages
{
    public class Confirm : DNP3Function
    {
        private MessageHeaderBuilder headerBuilder;
        public Confirm(DNP3ApplicationObjectParameters commandParameters) : base(commandParameters)
        {
            headerBuilder = new MessageHeaderBuilder();
        }

        public override byte[] PackRequest()
        {
            byte[] message = new byte[15];

            CommandParameters.Length = 0x08;
            Buffer.BlockCopy(headerBuilder.Build(CommandParameters), 0, message, 0, 10);
            message[10] = CommandParameters.TransportControl;
            message[11] = CommandParameters.AplicationControl;
            message[12] = CommandParameters.FunctionCode;

            ushort crc = 0;
            for (int i = 10; i < 13; i++)
            {
                CrcCalculator.computeCRC(message[i], ref crc);
            }
            crc = (ushort)(~crc);

            Buffer.BlockCopy(BitConverter.GetBytes(crc), 0, message, 13, 2);

            return message;
        }
    }
}
