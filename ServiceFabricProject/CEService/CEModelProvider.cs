using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CE;
using CE.Common;
using Core.Common.Contracts;
using FTN.Common;
using FTN.Services.NetworkModelService;
using SF.Common.Proxies;

namespace CEService
{
    public class CEModelProvider : IModelUpdateAsync
    {
        private StatelessServiceContext _context;

        public static CEWorker cEWorker;

        public CEModelProvider(StatelessServiceContext context)
        {
            _context = context;
        }

        public async Task<bool> ModelUpdate(AffectedEntities model)
        {
            var proxy = new NetworkModelServiceProxy();
            Console.WriteLine("New update request!");
            if (CeDataBase.Model == null)
                CeDataBase.Model = new Dictionary<DMSType, Container>();
            model.Insert = model.Insert.Where(x => GetDMSType(x) == DMSType.ASYNCHRONOUSMACHINE).ToList();
            model.Update = model.Update.Where(x => GetDMSType(x) == DMSType.ASYNCHRONOUSMACHINE).ToList();
            model.Delete = model.Delete.Where(x => GetDMSType(x) == DMSType.ASYNCHRONOUSMACHINE).ToList();
            if (model.Insert.Count > 0)
            {
                var dataInsert = await proxy.GetValues(model.Insert);
                foreach (var item in dataInsert)
                {
                    var dmsType = GetDMSType(item.GID);
                    if (!CeDataBase.Model.ContainsKey(dmsType))
                        CeDataBase.Model.Add(dmsType, new Container());
                    CeDataBase.Model[dmsType].AddEntity(item);
                }
            }

            if (model.Update.Count > 0)
            {
                var dataUpdate = await proxy.GetValues(model.Update);
                foreach (var item in dataUpdate)
                {
                    var dmsType = GetDMSType(item.GID);
                    if (!CeDataBase.Model.ContainsKey(dmsType))
                        CeDataBase.Model.Add(dmsType, new Container());
                    CeDataBase.Model[dmsType].RemoveEntity(item.GID);
                    CeDataBase.Model[dmsType].AddEntity(item);
                }
            }

            if (model.Delete.Count > 0)
            {
                var dataDelete = await proxy.GetValues(model.Delete);
                foreach (var item in dataDelete)
                {
                    var dmsType = GetDMSType(item.GID);
                    if (!CeDataBase.Model.ContainsKey(dmsType))
                        CeDataBase.Model.Add(dmsType, new Container());
                    CeDataBase.Model[dmsType].RemoveEntity(item.GID);

                }
            }
            EnList();
            cEWorker.TPoints = GetPointsCount(CeDataBase.Model);
            return true;
        }
        public void EnList()
        {
            /*TransactionManagerProxy proxyForTM = new TransactionManagerProxy();
            proxyForTM.Enlist();*/
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
