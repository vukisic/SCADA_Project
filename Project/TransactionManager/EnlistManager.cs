using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TMContracts;

namespace TransactionManager
{
    public class EnlistManager : IEnlistManager
    {
        public void EndEnlist(bool isSuccessful)
        {
            Console.WriteLine("NMS ended transaction.");
            StepsOfTransaction();
        }
        public void StepsOfTransaction()
        {
            Thread.Sleep(2000);
            bool everyServiceIsPrepared = false;
            bool everyServiceCommited = false;

            SCADATransactionProxy proxyForScada = new SCADATransactionProxy();
            everyServiceIsPrepared = proxyForScada.Prepare();

            CalculationEngineTransactionProxy proxyForCE = new CalculationEngineTransactionProxy();
            everyServiceIsPrepared = proxyForCE.Prepare();

            NMSProxy proxyForNms = new NMSProxy();
            everyServiceIsPrepared = proxyForNms.Prepare();


            if (everyServiceIsPrepared)
            {
                Console.WriteLine("Every service is prepared to commit..calling commit");
                everyServiceCommited = proxyForScada.Commit();
                everyServiceCommited = proxyForCE.Commit();
                everyServiceCommited = proxyForNms.Commit();
            }

            if (!everyServiceCommited)
            {
                Console.WriteLine("ERROR..requesting rollback");
                proxyForScada.Rollback();
                proxyForCE.Rollback();
                proxyForNms.Rollback();
            }
            Console.WriteLine("Every service commited! MODEL UPDATED!");
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
