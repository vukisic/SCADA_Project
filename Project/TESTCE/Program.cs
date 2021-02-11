using System;

namespace CE
{
    internal class Program
    {
        private static void Main(string[] args)
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
