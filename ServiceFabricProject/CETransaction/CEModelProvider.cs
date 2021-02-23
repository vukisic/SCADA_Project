using System;
using System.Collections.Generic;
using CE.Common;
using FTN.Common;
using FTN.Services.NetworkModelService;
using TMContracts;

namespace CETransaction
{
    public class CEModelProvider : IModelUpdate
    {
        public bool ModelUpdate(Dictionary<DMSType, Container> model)
        {

            Console.WriteLine("New update request!");
            //dobio si model, javi se TM-u da ucestvujes u transakciji
            EnList();
            CeDataBase.Model = model;
            return true;
        }
        public void EnList()
        {
            TransactionManagerProxy proxyForTM = new TransactionManagerProxy();
            proxyForTM.Enlist();

        }
    }
}
