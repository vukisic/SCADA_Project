using System;
using System.Collections.Generic;
using CE.Common;
using FTN.Common;
using FTN.ServiceContracts;
using FTN.Services.NetworkModelService;
using TMContracts;

namespace CETransaction
{
    public class CEModelProvider : IModelUpdate
    {
        public bool ModelUpdate(AffectedEntities model)
        {

            Console.WriteLine("New update request!");
            NetworkModelGDAProxy proxy = new NetworkModelGDAProxy("NetworkModelGDAEndpoint");

            if (CeDataBase.Model == null)
                CeDataBase.Model = new Dictionary<DMSType, Container>();
            if(model.Insert.Count > 0)
            {
                var dataInsert = proxy.GetValues(model.Insert);
                foreach (var item in dataInsert)
                {
                    var dmsType = GetDMSType(item.GID);
                    if (!CeDataBase.Model.ContainsKey(dmsType))
                        CeDataBase.Model.Add(dmsType, new Container());
                    CeDataBase.Model[dmsType].AddEntity(item);
                }
            }
           
            if(model.Update.Count > 0)
            {
                var dataUpdate = proxy.GetValues(model.Update);
                foreach (var item in dataUpdate)
                {
                    var dmsType = GetDMSType(item.GID);
                    if (!CeDataBase.Model.ContainsKey(dmsType))
                        CeDataBase.Model.Add(dmsType, new Container());
                    CeDataBase.Model[dmsType].RemoveEntity(item.GID);
                    CeDataBase.Model[dmsType].AddEntity(item);
                }
            }
            
            if(model.Delete.Count > 0)
            {
                var dataDelete = proxy.GetValues(model.Delete);
                foreach (var item in dataDelete)
                {
                    var dmsType = GetDMSType(item.GID);
                    if (!CeDataBase.Model.ContainsKey(dmsType))
                        CeDataBase.Model.Add(dmsType, new Container());
                    CeDataBase.Model[dmsType].RemoveEntity(item.GID);

                }
            }
           

            //dobio si model, javi se TM-u da ucestvujes u transakciji
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
