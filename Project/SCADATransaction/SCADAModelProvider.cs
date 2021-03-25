using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTN.Common;
using FTN.ServiceContracts;
using FTN.Services.NetworkModelService;
using SCADA.Common;
using SCADA.Common.Proxies;
using SCADA.Common.ScadaServices;
using TMContracts;

namespace SCADATransaction
{
    public class SCADAModelProvider : IModelUpdate
    {
        public bool ModelUpdate(AffectedEntities model)
        {
            Console.WriteLine("New update request!");
            ScadaStorageProxy proxy = ScadaProxyFactory.Instance().ScadaStorageProxy();
            NetworkModelGDAProxy nms = new NetworkModelGDAProxy("NetworkModelGDAEndpoint");
            var cimModel = proxy.GetCimModel();
            if (cimModel == null)
                cimModel = new Dictionary<DMSType, Container>();

            model.Insert = model.Insert.Where(x => this.GetDMSType(x) == DMSType.ANALOG || this.GetDMSType(x) == DMSType.DISCRETE || 
                                                this.GetDMSType(x) == DMSType.BREAKER || this.GetDMSType(x) == DMSType.DISCONNECTOR).ToList();
            model.Update = model.Update.Where(x => this.GetDMSType(x) == DMSType.ANALOG || this.GetDMSType(x) == DMSType.DISCRETE ||
                                                this.GetDMSType(x) == DMSType.BREAKER || this.GetDMSType(x) == DMSType.DISCONNECTOR).ToList();
            model.Delete = model.Delete.Where(x => this.GetDMSType(x) == DMSType.ANALOG || this.GetDMSType(x) == DMSType.DISCRETE ||
                                                this.GetDMSType(x) == DMSType.BREAKER || this.GetDMSType(x) == DMSType.DISCONNECTOR).ToList();

            if(model.Insert.Count > 0)
            {
                var dataInsert = nms.GetValues(model.Insert);
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
                var dataUpdate = nms.GetValues(model.Update);
                foreach (var item in dataUpdate)
                {
                    var dmsType = GetDMSType(item.GID);
                    if (!cimModel.ContainsKey(dmsType))
                        cimModel.Add(dmsType, new Container());
                    cimModel[dmsType].RemoveEntity(item.GID);
                    cimModel[dmsType].AddEntity(item);
                }
            }
            if(model.Delete.Count> 0)
            {
                var dataDelete = nms.GetValues(model.Delete);
                foreach (var item in dataDelete)
                {
                    var dmsType = GetDMSType(item.GID);
                    if (!cimModel.ContainsKey(dmsType))
                        cimModel.Add(dmsType, new Container());
                    cimModel[dmsType].RemoveEntity(item.GID);
                }
            }
           
            proxy.SetCimModel(cimModel);
            //dobio si model, javi se da ucestvujes u transakciji
            EnList();
            return true;
        }

        public DMSType GetDMSType(long globalId)
        {
            return (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(globalId);
        }

        public void EnList()
        {
            TransactionManagerProxy proxyForTM = new TransactionManagerProxy();
            proxyForTM.Enlist();
        }
    }
}
