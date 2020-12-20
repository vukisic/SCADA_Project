using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SCADA.Common.Connection;
using SCADA.Common.DataModel;
using SCADA.Common.Messaging;
using SCADA.Common.Messaging.Messages;
using SCADA.Common.Messaging.Parameters;

namespace FEP
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "FrotEndProcessor";
            TCPConnection connection = new TCPConnection();
            connection.Connect();

            var param = new DNP3ApplicationObjectParameters(0xc4, (byte)DNP3FunctionCode.DIRECT_OPERATE, (ushort)TypeField.BINARY_COMMAND, 0x28, 0x01, 0, 1, 0, 1, 2, 0xc1);
            param = new DNP3ApplicationObjectParameters(0xc4, (byte)DNP3FunctionCode.DIRECT_OPERATE, (ushort)TypeField.ANALOG_OUTPUT_16BIT, 0x28, 0x01, 0, 10, 0, 1, 2, 0xc1);
            while (true)
            {
                Thread.Sleep(5000);
                var result = DNP3FunctionFactory.CreateFunction(param);
                byte[] m = result.PackRequest();
                connection.Send(m);
            }
        }
    }
}
