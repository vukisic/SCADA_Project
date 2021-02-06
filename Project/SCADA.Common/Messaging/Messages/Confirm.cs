using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common.DataModel;
using SCADA.Common.Messaging.Parameters;

namespace SCADA.Common.Messaging.Messages
{
    public class Confirm : DNP3Function
    {
        private MessageHeaderBuilder headerBuilder;
        public Confirm(DNP3CommandParameters commandParameters) : base(commandParameters)
        {
            headerBuilder = new MessageHeaderBuilder();
        }

        public override byte[] PackRequest()
        {
            byte[] message = new byte[15];

            DNP3ConfirmCommandParamters commandParam = (DNP3ConfirmCommandParamters)CommandParameters;

            CommandParameters.Length = 0x08;
            Buffer.BlockCopy(headerBuilder.Build(CommandParameters), 0, message, 0, 10);
            message[10] = commandParam.TransportControl;
            message[11] = commandParam.AplicationControl;
            message[12] = commandParam.FunctionCode;

            ushort crc = 0;
            for (int i = 10; i < 13; i++)
            {
                CrcCalculator.computeCRC(message[i], ref crc);
            }
            crc = (ushort)(~crc);

            Buffer.BlockCopy(BitConverter.GetBytes(crc), 0, message, 13, 2);

            return message;
        }

        public override Dictionary<Tuple<RegisterType, int>, BasePoint> PareseResponse(byte[] response)
        {
            throw new NotImplementedException();
        }
    }
}
