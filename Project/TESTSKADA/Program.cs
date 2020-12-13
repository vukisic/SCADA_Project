using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCADATransaction;
using TMContracts;

namespace TESTSKADA
{
	class Program
	{
		static void Main(string[] args)
		{
            Console.WriteLine("SCADA started working..");

            SCADAServer scada = new SCADAServer();
            scada.OpenModel();
            scada.OpenTransaction();

            Console.ReadLine();
		}
	}
}
