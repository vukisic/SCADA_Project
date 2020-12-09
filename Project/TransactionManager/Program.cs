using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMContracts;

namespace TransactionManager
{
	class Program
	{
		static void Main(string[] args)
		{
            bool everyServiceIsPrepared = false;
            bool everyServiceCommited = false;

            Console.WriteLine("Trasanction Manager starter working..");

            EnlistServiceHost host = new EnlistServiceHost();
            host.Open();

            SCADATransactionProxy proxyForScada = new SCADATransactionProxy();
            everyServiceIsPrepared = proxyForScada.Prepare();

            CalculationEngineTransactionProxy proxyForCE = new CalculationEngineTransactionProxy();
            everyServiceIsPrepared = proxyForCE.Prepare();

            NMSProxy proxyForNms = new NMSProxy();
            everyServiceIsPrepared = proxyForNms.Prepare();


            if(everyServiceIsPrepared)
            {
                Console.WriteLine("Every service is prepared to commit..calling commit");
                everyServiceCommited = proxyForScada.Commit();
                everyServiceCommited = proxyForCE.Commit();
                everyServiceCommited = proxyForNms.Commit();
            }

            if(!everyServiceCommited)
            {
                Console.WriteLine("ERROR..requesting rollback");
                proxyForScada.Rollback();
                proxyForCE.Rollback();
                proxyForNms.Rollback();
            }
            Console.WriteLine("Every service commited! MODEL UPDATED!");

            Console.ReadLine();
        }
	}
}
