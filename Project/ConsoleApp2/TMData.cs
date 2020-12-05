using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMContracts;

namespace ConsoleApp2
{
    public class TMData
    {
        public static SynchronizedCollection<ITransactionSteps> CurrentlyEnlistedServices = new SynchronizedCollection<ITransactionSteps>();

        public static List<ITransactionSteps> CompleteEnlistedServices = new List<ITransactionSteps>();
    }
}
