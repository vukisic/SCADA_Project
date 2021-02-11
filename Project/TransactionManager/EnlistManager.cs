using System;
using System.Threading;
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
            bool isPreparedSCADA = false;
            bool isPreparedNMS = false;
            bool isPreparedCE = false;
            bool commitedSCADA = false;
            bool commitedNMS = false;
            bool commitedCE = false;

            SCADATransactionProxy proxyForScada = new SCADATransactionProxy();
            isPreparedSCADA = proxyForScada.Prepare();

            CalculationEngineTransactionProxy proxyForCE = new CalculationEngineTransactionProxy();
            isPreparedCE = proxyForCE.Prepare();

            NMSProxy proxyForNms = new NMSProxy();
            isPreparedNMS = proxyForNms.Prepare();


            if (isPreparedSCADA && isPreparedCE && isPreparedNMS)
            {
                Console.WriteLine("Every service is prepared to commit..calling commit");
                commitedSCADA = proxyForScada.Commit();
                commitedNMS = proxyForCE.Commit();
                commitedCE = proxyForNms.Commit();
            }

            if (!(commitedSCADA && commitedCE && commitedNMS))
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
