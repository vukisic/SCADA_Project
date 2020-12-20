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
using SCADATransaction;
using TMContracts;

namespace NDS
{
	class Program
	{
		static void Main(string[] args)
		{
            Console.Title = "NetworkDynamicService";
            Console.WriteLine("SCADA started working..");

            SCADAServer scada = new SCADAServer();
            scada.OpenModel();
            scada.OpenTransaction();

            Console.ReadLine();
		}
	}
}
