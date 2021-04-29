using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data;
using SF.Common.Proxies;
using TMContracts;

namespace TransactionManager
{
    public class EnlistManager
    {
        private IReliableStateManager _manager;
        public EnlistManager(IReliableStateManager stateManager)
        {
            _manager = stateManager;
        }
        public async Task EndEnlist(bool isSuccessful)
        {
            Console.WriteLine("NMS ended transaction.");
            await StepsOfTransaction();
        }
        public async Task StepsOfTransaction()
        {
            Thread.Sleep(2000);
            bool isPreparedSCADA = false;
            bool isPreparedNMS = false;
            bool isPreparedCE = false;
            bool commitedSCADA = false;
            bool commitedNMS = false;
            bool commitedCE = false;

            NetworkModelServiceTransactionProxy proxyForNms = new NetworkModelServiceTransactionProxy(ConfigurationManager.AppSettings["NMST"]);
            isPreparedNMS = await proxyForNms.Prepare();

            NDSTransactionProxy proxyForScada = new NDSTransactionProxy(ConfigurationManager.AppSettings["SCADAT"]);
            isPreparedSCADA = await proxyForScada.Prepare();

            CETransactionProxy proxyForCE = new CETransactionProxy(ConfigurationManager.AppSettings["CET"]);
            isPreparedCE = await proxyForCE.Prepare();

            if (isPreparedSCADA && isPreparedCE && isPreparedNMS)
            {
                Console.WriteLine("Every service is prepared to commit..calling commit");
                commitedSCADA = await proxyForScada.Commit();
                commitedNMS = await proxyForCE.Commit();
                commitedCE = await proxyForNms.Commit();
            }

            if (!(commitedSCADA && commitedCE && commitedNMS))
            {
                Console.WriteLine("ERROR..requesting rollback");
                await proxyForScada.Rollback();
                await proxyForCE.Rollback();
                await proxyForNms.Rollback();
            }
            Console.WriteLine("Every service commited! MODEL UPDATED!");
        }
        public async Task Enlist()
        {
            Console.WriteLine("New service enter transaction.");
        }

        public async Task<bool> StartEnlist()
        {
            Console.WriteLine("NMS started transaction.");
            return true;
        }
    }
}
