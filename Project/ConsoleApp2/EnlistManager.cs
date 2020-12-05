using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
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
            OperationContext context = OperationContext.Current;

            var service = context.GetCallbackChannel<ITransactionSteps>();
            TMData.CurrentlyEnlistedServices.Add(service);
        }
    }
}
