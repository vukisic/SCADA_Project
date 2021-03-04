using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CE.Common;
using Core.Common.Contracts;
using FTN.Common;
using FTN.Services.NetworkModelService;

namespace CEService
{
    public class CEModelProvider : IModelUpdateAsync
    {
        private StatelessServiceContext _context;
        public CEModelProvider(StatelessServiceContext context)
        {
            _context = context;
        }

        public Task<bool> ModelUpdate(Dictionary<DMSType, Container> model)
        {
            Console.WriteLine("New update request!");
            //dobio si model, javi se TM-u da ucestvujes u transakciji
            EnList();
            CeDataBase.Model = model;

            return Task.FromResult<bool>(true);
        }
        public void EnList()
        {
            /*TransactionManagerProxy proxyForTM = new TransactionManagerProxy();
            proxyForTM.Enlist();*/
        }
    }
}
