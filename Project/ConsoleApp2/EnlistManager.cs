using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using TMContracts;

namespace ConsoleApp2
{
    public class EnlistManager : IEnlistManager
    {
        public void EndEnlist(bool isSuccessful)
        {
            if (!isSuccessful)
            {
                TMData.CurrentlyEnlistedServices = new SynchronizedCollection<ITransactionSteps>();
                return;
            }
            TMData.CompleteEnlistedServices = new List<ITransactionSteps>(TMData.CurrentlyEnlistedServices);
            TMData.CurrentlyEnlistedServices = new SynchronizedCollection<ITransactionSteps>();

            TransactionSteps.BeginTransaction();
        }

        public void Enlist()
        {
            Console.WriteLine("Javio se servis za transakciju.");
            //TMData.CurrentlyEnlistedServices.Add(service);

        }
    }
}
