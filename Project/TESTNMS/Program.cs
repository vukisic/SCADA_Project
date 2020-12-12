using System;
using System.Collections.Generic;
using NMSTransaction;
using TMContracts;

namespace TESTNMS
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("NMS started working..");

            NMSServer service = new NMSServer();
            service.Open();

            TransactionManagerProxy proxyForTM = new TransactionManagerProxy();

            //Zapocni transakciju i prijavi se na nju
            bool pom = false;
            while (!pom)
            {
                pom = proxyForTM.StartEnlist();
            }

            proxyForTM.Enlist();

            //Posalji Scadi i CEu novi model
            NMSSCADAProxy proxyForScada = new NMSSCADAProxy();
            NMSCalculationEngineProxy proxyForCE = new NMSCalculationEngineProxy();

            Dictionary<string, List<string>> par = new Dictionary<string, List<string>>();
            par.Add("Marko", new List<string>());

            bool success = false;
            if (proxyForScada.ModelUpdate(par))
                success = true;

            if (proxyForCE.ModelUpdate(par))
                success = true;

            proxyForTM.EndEnlist(success);

            Console.ReadLine();
        }
    }
}
