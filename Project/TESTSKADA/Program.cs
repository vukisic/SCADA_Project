using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SCADA.Common.Connection;
using SCADA.Common.DataModel;
using SCADA.Common.Messaging.Messages;
using SCADA.Common.Messaging.Parameters;
using SCADATransaction;
using TMContracts;

namespace NDS
{
	class Program
	{
		static void Main(string[] args)
		{
            TCPConnection connection = new TCPConnection();
            connection.Connect();

            DNP3ApplicationObjectParameters dnp3 = new DNP3ApplicationObjectParameters(0xC, (byte)DNP3FunctionCode.READ, 0x3C01, 0x6, 0, 0, 0, 0, 1, 2, 0xd3);
            ReadClass0 rc0 = new ReadClass0(dnp3);
            while(true)
            {
                Thread.Sleep(5000);
                byte[] m = rc0.PackRequest();
                connection.Send(m);
            }


            Console.Title = "NetworkDynamicService";
            Console.WriteLine("SCADA started working..");

            SCADAServer scada = new SCADAServer();
            scada.OpenModel();
            scada.OpenTransaction();

            Console.ReadLine();
		}
	}
}
