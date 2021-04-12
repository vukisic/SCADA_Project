using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Contracts;
using FTN.Common;
using FTN.Services.NetworkModelService;
using SF.Common.Proxies;

namespace NDSService
{
    public class NDSModelProvider : IModelUpdateAsync
    {
        private StatelessServiceContext _context;
        public NDSModelProvider(StatelessServiceContext context)
        {
            _context = context;
        }
        public async Task<bool> ModelUpdate(AffectedEntities model)
        {
            var nms = new NetworkModelServiceProxy();
            ScadaStorageProxy storage = new ScadaStorageProxy();
            var cimModel = await storage.GetCimModel();
            if (cimModel == null)
                cimModel = new Dictionary<DMSType, Container>();

            model.Insert = model.Insert.Where(x => this.GetDMSType(x) == DMSType.ANALOG || this.GetDMSType(x) == DMSType.DISCRETE ||
                                                this.GetDMSType(x) == DMSType.BREAKER || this.GetDMSType(x) == DMSType.DISCONNECTOR).ToList();
            model.Update = model.Update.Where(x => this.GetDMSType(x) == DMSType.ANALOG || this.GetDMSType(x) == DMSType.DISCRETE ||
                                                this.GetDMSType(x) == DMSType.BREAKER || this.GetDMSType(x) == DMSType.DISCONNECTOR).ToList();
            model.Delete = model.Delete.Where(x => this.GetDMSType(x) == DMSType.ANALOG || this.GetDMSType(x) == DMSType.DISCRETE ||
                                                this.GetDMSType(x) == DMSType.BREAKER || this.GetDMSType(x) == DMSType.DISCONNECTOR).ToList();

            if (model.Insert.Count > 0)
            {
                var dataInsert = await nms.GetValues(model.Insert);
                foreach (var item in dataInsert)
                {
                    var dmsType = GetDMSType(item.GID);
                    if (!cimModel.ContainsKey(dmsType))
                        cimModel.Add(dmsType, new Container());
                    cimModel[dmsType].AddEntity(item);
                }
            }
            if (model.Update.Count > 0)
            {
                var dataUpdate = await nms.GetValues(model.Update);
                foreach (var item in dataUpdate)
                {
                    var dmsType = GetDMSType(item.GID);
                    if (!cimModel.ContainsKey(dmsType))
                        cimModel.Add(dmsType, new Container());
                    cimModel[dmsType].RemoveEntity(item.GID);
                    cimModel[dmsType].AddEntity(item);
                }
            }
            if (model.Delete.Count > 0)
            {
                var dataDelete = await nms.GetValues(model.Delete);
                foreach (var item in dataDelete)
                {
                    var dmsType = GetDMSType(item.GID);
                    if (!cimModel.ContainsKey(dmsType))
                        cimModel.Add(dmsType, new Container());
                    cimModel[dmsType].RemoveEntity(item.GID);
                }
            }

            await storage.SetCimModel(cimModel);
            EnList();
            return true;
        }

        public void EnList()
        {
            // Ostaviti zakomentarisano
            /*TransactionManagerProxy proxyForTM = new TransactionManagerProxy();
            proxyForTM.Enlist();*/
        }

        public DMSType GetDMSType(long globalId)
        {
            return (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(globalId);
        }
    }
}
