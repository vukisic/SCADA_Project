using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTN.Common;
using FTN.Services.NetworkModelService;
using SCADA.Common;
using SCADA.Services;
using TMContracts;

namespace SCADATransaction
{
    public class SCADAModelProvider : IModelUpdate
    {
        public bool ModelUpdate(Dictionary<DMSType, Container> model)
        {
            Console.WriteLine("New update request!");
            ScadaStorageProxy proxy = new ScadaStorageProxy();
            proxy.SetCimModel(model);
            //dobio si model, javi se da ucestvujes u transakciji
            EnList();
            return true;
        }
        public void EnList()
        {
            TransactionManagerProxy proxyForTM = new TransactionManagerProxy();
            proxyForTM.Enlist();
        }
    }
}
