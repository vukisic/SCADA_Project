using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Contracts;
using FTN.Common;
using FTN.Services.NetworkModelService;
using SF.Common;
using SF.Common.Proxies;

namespace CEDynamicsService
{
    public class CEModelProvider : IModelUpdateAsync
    {
        private StatelessServiceContext _context;

        public CEModelProvider(StatelessServiceContext context)
        {
            _context = context;
        }

        public async Task<bool> ModelUpdate(AffectedEntities model)
        {
            var proxy = new NetworkModelServiceProxy(ConfigurationReader.ReadValue(_context,"Settings","NMS")?? "net.tcp://localhost:22330/NetworkModelServiceSF");
            var storage = new CEStorageProxy(ConfigurationReader.ReadValue(_context,"Settings","CES")?? "fabric:/ServiceFabricApp/CEStorageService");
            var ceModel = await storage.GetModel();
            if (ceModel == null)
                ceModel = new Dictionary<DMSType, Container>();
            Console.WriteLine("New update request!");
            model.Insert = model.Insert.Where(x => GetDMSType(x) == DMSType.ASYNCHRONOUSMACHINE).ToList();
            model.Update = model.Update.Where(x => GetDMSType(x) == DMSType.ASYNCHRONOUSMACHINE).ToList();
            model.Delete = model.Delete.Where(x => GetDMSType(x) == DMSType.ASYNCHRONOUSMACHINE).ToList();
            if (model.Insert.Count > 0)
            {
                var dataInsert = await proxy.GetValues(model.Insert);
                foreach (var item in dataInsert)
                {
                    var dmsType = GetDMSType(item.GID);
                    if (!ceModel.ContainsKey(dmsType))
                        ceModel.Add(dmsType, new Container());
                    ceModel[dmsType].AddEntity(item);
                }
            }

            if (model.Update.Count > 0)
            {
                var dataUpdate = await proxy.GetValues(model.Update);
                foreach (var item in dataUpdate)
                {
                    var dmsType = GetDMSType(item.GID);
                    if (!ceModel.ContainsKey(dmsType))
                        ceModel.Add(dmsType, new Container());
                    ceModel[dmsType].RemoveEntity(item.GID);
                    ceModel[dmsType].AddEntity(item);
                }
            }

            if (model.Delete.Count > 0)
            {
                var dataDelete = await proxy.GetValues(model.Delete);
                foreach (var item in dataDelete)
                {
                    var dmsType = GetDMSType(item.GID);
                    if (!ceModel.ContainsKey(dmsType))
                        ceModel.Add(dmsType, new Container());
                    ceModel[dmsType].RemoveEntity(item.GID);

                }
            }

            await storage.SetTransactionalModel(ceModel);
            EnList();
            return true;
        }
        public void EnList()
        {
            TransactionManagerServiceProxy proxyForTM = new TransactionManagerServiceProxy(ConfigurationReader.ReadValue(_context,"Settings","TM")?? "fabric:/ServiceFabricApp/TransactionManagerService");
            proxyForTM.Enlist().GetAwaiter().GetResult();
        }

        private int GetPointsCount(Dictionary<DMSType, Container> collection)
        {
            if (!collection.ContainsKey(DMSType.ASYNCHRONOUSMACHINE))
                return 0;
            return collection[DMSType.ASYNCHRONOUSMACHINE] == null ? 0 : collection[DMSType.ASYNCHRONOUSMACHINE].Count;
        }

        public DMSType GetDMSType(long globalId)
        {
            return (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(globalId);
        }
    }
}
