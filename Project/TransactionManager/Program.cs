using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMContracts;

namespace TransactionManager
{
	class Program
	{
		static void Main(string[] args)
		{
            Console.Title = "TM";

            Console.WriteLine("Trasanction Manager starter working..");

            EnlistServiceHost host = new EnlistServiceHost();
            host.Open();

            Console.ReadLine();
        }
	}
}
