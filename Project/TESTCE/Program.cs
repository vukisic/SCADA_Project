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
            Console.Title = "CE";
            Console.WriteLine("SCADA started working..");

            CEServer ce = new CEServer();
            ce.OpenModel();
            ce.OpenTransaction();

            Console.ReadLine();
        }
    }
}
