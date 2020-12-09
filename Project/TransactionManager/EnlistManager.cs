using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMContracts;

namespace TransactionManager
{
    public class EnlistManager : IEnlistManager
    {
        public void EndEnlist(bool isSuccessful)
        {
            Console.WriteLine("NMS ended transaction.");
        }

        public void Enlist()
        {
            Console.WriteLine("New service enter transaction.");
        }

        public bool StartEnlist()
        {
            Console.WriteLine("NMS started transaction.");
            return true;
        }
    }
}
