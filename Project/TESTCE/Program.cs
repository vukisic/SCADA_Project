using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CETransaction;
using TMContracts;

namespace TESTCE
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("SCADA started working..");

            CEServer ce = new CEServer();
            ce.OpenModel();
            ce.OpenTransaction();

            TransactionManagerProxy proxyForTM = new TransactionManagerProxy();
            proxyForTM.Enlist();


            Console.ReadLine();
        }
    }
}
