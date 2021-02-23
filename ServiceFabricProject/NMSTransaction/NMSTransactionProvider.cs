using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TMContracts;

namespace NMSTransaction
{
    public class NMSTransactionProvider : ITransactionSteps
    {
        public bool Prepare()
        {
            Console.WriteLine("Prepared? YES");
            return true;
        }

        public bool Commit()
        {
            Console.WriteLine("Commited? YES");
            return true;
        }

        public void Rollback()
        {
            Console.WriteLine("Request for rollback!");
        }
    }
}
