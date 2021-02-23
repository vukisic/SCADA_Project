using System;

namespace TransactionManager
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.Title = "TM";

            Console.WriteLine("Trasanction Manager starter working..");

            EnlistServiceHost host = new EnlistServiceHost();
            host.Open();

            Console.ReadLine();
        }
    }
}
