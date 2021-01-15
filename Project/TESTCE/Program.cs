using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CETransaction;
using TMContracts;

namespace CE
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "CE";
            Console.WriteLine("CE started working..");

            CEServiceInvoker serviceInvoker = new CEServiceInvoker();
            serviceInvoker.Start();

            Console.ReadLine();

            serviceInvoker.Stop();
        }
    }
}
