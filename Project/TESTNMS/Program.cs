using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMContracts;

namespace TESTNMS
{
	class Program
	{
		static void Main(string[] args)
		{
            TestNMSClass test = new TestNMSClass();

            test.Initialize();

            //zapocni transakciju
            test.EnList();

            test.SendModelToScada();

            //svi servisi su se enlistovali, zapocni one tri metode
            test.EndEnList(true);

            Console.ReadLine();
		}
	}
}
