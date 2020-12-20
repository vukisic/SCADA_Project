using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common.Messaging.Parameters;

namespace SCADA.Common.Messaging.Messages
{
    public class WriteAnalogOutput : DNP3Function
    {
        private MessageHeaderBuilder headerBuilder;
        public WriteAnalogOutput(DNP3ApplicationObjectParameters commandParameters) : base(commandParameters)
        {
            headerBuilder = new MessageHeaderBuilder();
        }
        public override byte[] PackRequest()
        {
            byte[] dnp3Request = new byte[25];
            CommandParameters.Length = 18;
            Buffer.BlockCopy(headerBuilder.Build(CommandParameters), 0, dnp3Request, 0, 10);
            dnp3Request[10] = CommandParameters.TransportControl;
            dnp3Request[11] = CommandParameters.AplicationControl;
            dnp3Request[12] = CommandParameters.FunctionCode;
            dnp3Request[13] = BitConverter.GetBytes((short)CommandParameters.ObjectTypeField)[1];
            dnp3Request[14] = BitConverter.GetBytes((short)CommandParameters.ObjectTypeField)[0];
            dnp3Request[15] = CommandParameters.Qualifier;
            Buffer.BlockCopy(BitConverter.GetBytes(Convert.ToUInt16(CommandParameters.RangeField)), 0, dnp3Request, 16, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(Convert.ToUInt16(CommandParameters.Prefix)), 0, dnp3Request, 18, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(Convert.ToUInt16(CommandParameters.Value)), 0, dnp3Request, 20, 2);
            dnp3Request[22] = 0x00;

            ushort crc1 = 0;
            for (int i = 10; i < 23; i++)
            {
                CrcCalculator.computeCRC(dnp3Request[i], ref crc1);
            }
            crc1 = (ushort)(~crc1);

            Buffer.BlockCopy(BitConverter.GetBytes(crc1), 0, dnp3Request, 23, 2);

            return dnp3Request;
        }
    }
}
