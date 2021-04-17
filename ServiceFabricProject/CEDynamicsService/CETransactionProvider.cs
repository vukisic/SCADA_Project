using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Contracts;
using SF.Common;
using SF.Common.Proxies;

namespace CEDynamicsService
{
    public class CETransactionProvider : ITransactionStepsAsync
    {
        private StatelessServiceContext _context;

        public CETransactionProvider(StatelessServiceContext context)
        {
            _context = context;
        }

        public async Task<bool> Commit()
        {
            Console.WriteLine("Commited? YES");
            var storage = new CEStorageProxy(ConfigurationReader.ReadValue(_context, "Settings", "CES") ?? "fabric:/ServiceFabricApp/CEStorageService");
            var tModel = await storage.GetTransactionalModel();
            var count = tModel[FTN.Common.DMSType.ASYNCHRONOUSMACHINE].Count;
            await storage.SetModel(tModel);
            await storage.SetTransactionalModel(new Dictionary<FTN.Common.DMSType, FTN.Services.NetworkModelService.Container>());
            var ceService = new CEServiceProxy(ConfigurationReader.ReadValue(_context,"Settings","CEService") ?? "fabric:/ServiceFabricApp/CEService");
            await ceService.SetPoints(count);
            return true;
        }

        public Task<bool> Prepare()
        {
            Console.WriteLine("CE Prepared");
            return Task.FromResult<bool>(true);
        }

        public async Task Rollback()
        {
            Console.WriteLine("Request for rollback!");
            var storage = new CEStorageProxy(ConfigurationReader.ReadValue(_context, "Settings", "CES") ?? "fabric:/ServiceFabricApp/CEStorageService");
            await storage.SetTransactionalModel(new Dictionary<FTN.Common.DMSType, FTN.Services.NetworkModelService.Container>());
        }
    }
}
