using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Common.ServiceBus.Commands;
using Core.Common.ServiceBus.Dtos.Conversion;
using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using NServiceBus;
using TMContracts;

namespace FTN.Services.NetworkModelService
{
    public class NetworkModel
    {
        #region Fields
        private Dictionary<DMSType, Container> networkDataModel;
        private Dictionary<DMSType, Container> networkDataModelCopy;
        private Dictionary<long, long> GidHelper;
        private ModelResourcesDesc resourcesDescs;
        private string _tableName = "Delta";
        private AzureStorage storage;
        private IReliableStateManager _manager;
        #endregion
        public NetworkModel(IReliableStateManager stateManager)
        {
            _manager = stateManager;
            networkDataModel = new Dictionary<DMSType, Container>();
            networkDataModelCopy = new Dictionary<DMSType, Container>();
            resourcesDescs = new ModelResourcesDesc();
            storage = new AzureStorage(_tableName, false);
            GidHelper = new Dictionary<long, long>();
            GetDictionaries().GetAwaiter().GetResult();
            Initialize();
        }
       
        private async Task GetDictionaries()
        {
            try
            {
                var dict = await _manager.GetOrAddAsync<IReliableDictionary<string, Dictionary<DMSType, Container>>>("data");
                var gid = await _manager.GetOrAddAsync<IReliableDictionary<string, Dictionary<long, long>>>("gidData");
                using (var tx = _manager.CreateTransaction())
                {
                    var nm = await dict.TryGetValueAsync(tx, "NM");
                    if (nm.HasValue)
                        networkDataModel = nm.Value;
                    else
                        networkDataModel = new Dictionary<DMSType, Container>();

                    var nmc = await dict.TryGetValueAsync(tx, "NMC");
                    if (nmc.HasValue)
                        networkDataModelCopy = nmc.Value;
                    else
                        networkDataModelCopy = new Dictionary<DMSType, Container>();

                    var gidresult = await gid.TryGetValueAsync(tx, "GID");
                    if (gidresult.HasValue)
                        GidHelper = gidresult.Value;
                    else
                        GidHelper = new Dictionary<long, long>();
                    await tx.CommitAsync();
                }
            }
            catch { }
           
        }

        private async Task SetDictionaries()
        {
            try
            {
                var dict = await _manager.GetOrAddAsync<IReliableDictionary<string, Dictionary<DMSType, Container>>>("data");
                var gid = await _manager.GetOrAddAsync<IReliableDictionary<string, Dictionary<long, long>>>("gidData");
                using (var tx = _manager.CreateTransaction())
                {
                    await dict.ClearAsync();
                    await gid.ClearAsync();
                    await dict.SetAsync(tx, "NM", networkDataModel);
                    await dict.SetAsync(tx, "NMC", networkDataModelCopy);
                    await gid.SetAsync(tx, "GID", GidHelper);
                    await tx.CommitAsync();
                }
            }
            catch { }
            
        }

        #region Find

        public bool EntityExists(long globalId)
        {
            DMSType type = (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(globalId);

            if (ContainerExists(type))
            {
                Container container = GetContainer(type);

                if (container.EntityExists(globalId))
                {
                    return true;
                }
            }

            return false;
        }

        public IdentifiedObject GetEntity(long globalId)
        {
            if (EntityExists(globalId))
            {
                DMSType type = (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(globalId);
                IdentifiedObject io = GetContainer(type).GetEntity(globalId);

                return io;
            }
            else
            {
                string message = string.Format("Entity  (GID = 0x{0:x16}) does not exist.", globalId);
                throw new Exception(message);
            }
        }

        private bool ContainerExists(DMSType type)
        {
            if (networkDataModelCopy.ContainsKey(type))
            {
                return true;
            }

            return false;
        }

        private Container GetContainer(DMSType type)
        {
            if (ContainerExists(type))
            {
                return networkDataModelCopy[type];
            }
            else
            {
                string message = string.Format("Container does not exist for type {0}.", type);
                throw new Exception(message);
            }

        }

        #endregion Find

        public UpdateResult ApplyDelta(Delta delta)
        {
            GetDictionaries().GetAwaiter().GetResult();
            bool applyingStarted = false;
            bool transactionSucceded = true;
            UpdateResult updateResult = new UpdateResult();
            Delta newDelta = new Delta();
            GetShallowCopyModel();
            try
            {
                CommonTrace.WriteTrace(CommonTrace.TraceInfo, "Applying  delta to network model.");

                Dictionary<short, int> typesCounters = GetCounters();
                Dictionary<long, long> globalIdPairs = new Dictionary<long, long>();
                var result = GetAllCounters(typesCounters);
                Delta.Counter = result;
                delta.FixNegativeToPositiveIds(ref typesCounters, ref globalIdPairs);
                updateResult.GlobalIdPairs = globalIdPairs;
                delta.SortOperations();

                applyingStarted = true;

                foreach (ResourceDescription rd in delta.InsertOperations)
                {
                    // We need newRd because old-new mapping
                    InsertEntity(rd, out ResourceDescription newRd);
                    newDelta.AddDeltaOperation(DeltaOpType.Insert, newRd, true);
                }

                //transactionSucceded = TryApplyTransaction();

                if (transactionSucceded)
                {
                    MergeModelsFinal();
                }

            }
            catch (Exception ex)
            {
                string message = string.Format("Applying delta to network model failed. {0}.", ex.Message);
                CommonTrace.WriteTrace(CommonTrace.TraceError, message);
                applyingStarted = false;
                updateResult.Result = ResultType.Failed;
                updateResult.Message = message;
            }
            finally
            {
                if (applyingStarted && transactionSucceded)
                {
                    if (newDelta.InsertOperations.Count > 0)
                        SaveDelta(delta);
                }
                else
                {
                    RestoreModel();
                }

                if (updateResult.Result == ResultType.Succeeded)
                {
                    string mesage = "Applying delta to network model successfully finished.";
                    CommonTrace.WriteTrace(CommonTrace.TraceInfo, mesage);
                    updateResult.Message = mesage;
                }
            }

            return updateResult;
        }
      
		private void InsertEntity(ResourceDescription rd, out ResourceDescription newRd)
        {
            newRd = rd;
            if (rd == null)
            {
                CommonTrace.WriteTrace(CommonTrace.TraceVerbose, "Insert entity is not done because update operation is empty.");
                throw new Exception();
            }

            long globalId = rd.Id;

            CommonTrace.WriteTrace(CommonTrace.TraceInfo, "Inserting entity with GID ({0:x16}).", globalId);

            // check if mapping for specified global id already exists			
            if (this.EntityExists(globalId))
            {
                string message = string.Format("Failed to insert entity because entity with specified GID ({0:x16}) already exists in network model.", globalId);
                CommonTrace.WriteTrace(CommonTrace.TraceError, message);
                throw new Exception(message);
            }

            try
            {
                // find type
                DMSType type = (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(globalId);

                //check mrid already exist
                // if exist made mapping old-new mrids
                if (CheckMridExist(type, rd))
                {
                    var typedCollection = networkDataModelCopy[type];
                    var objMrid = rd.Properties.Single(x => x.Id == ModelCode.IDOBJ_MRID);
                    var result = typedCollection.Entities.Values.SingleOrDefault(x => x.MRID == objMrid.PropertyValue.StringValue);
                    GidHelper[globalId] = result.GID;
                    return;
                }

                Container container = null;

                // get container or create container 
                if (ContainerExists(type))
                {
                    container = GetContainer(type);
                }
                else
                {
                    container = new Container();
                    networkDataModelCopy.Add(type, container);
                }

                // create entity and add it to container
                IdentifiedObject io = container.CreateEntity(globalId);

                // apply properties on created entity
                if (rd.Properties != null)
                {
                    foreach (Property property in rd.Properties)
                    {
                        // globalId must not be set as property
                        if (property.Id == ModelCode.IDOBJ_GID)
                        {
                            continue;
                        }

                        if (property.Type == PropertyType.Reference)
                        {

                            // if property is a reference to another entity 
                            long targetGlobalId = property.AsReference();
                            if (GidHelper.ContainsKey(targetGlobalId))
                            {
                                //Update old-new mrids
                                targetGlobalId = GidHelper[targetGlobalId];
                                newRd.Properties.Single(x => x.Id == property.Id).SetValue(targetGlobalId);
                            }
                            if (targetGlobalId != 0)
                            {

                                if (!EntityExists(targetGlobalId))
                                {
                                    string message = string.Format("Failed to get target entity with GID: 0x{0:X16}. {1}", targetGlobalId);
                                    throw new Exception(message);
                                }

                                // get referenced entity for update
                                IdentifiedObject targetEntity = GetEntity(targetGlobalId);
                                targetEntity.AddReference(property.Id, io.GID);
                            }

                            io.SetProperty(property);
                        }
                        else
                        {
                            io.SetProperty(property);
                        }
                    }
                }

                CommonTrace.WriteTrace(CommonTrace.TraceVerbose, "Inserting entity with GID ({0:x16}) successfully finished.", globalId);
            }
            catch (Exception ex)
            {
                string message = string.Format("Failed to insert entity (GID = 0x{0:x16}) into model. {1}", rd.Id, ex.Message);
                CommonTrace.WriteTrace(CommonTrace.TraceError, message);
                throw new Exception(message);
            }
        }

        private void Initialize()
        {
            List<Delta> result = ReadAllDeltas();

            if (result.Count <= 0)
            {
                return;
            }

            foreach (var delta in result)
            {
                ResourceDescription tempRd = new ResourceDescription();
                GetShallowCopyModel();
                try
                {
                    foreach (ResourceDescription rd in delta.InsertOperations)
                    {
                        InsertEntity(rd, out tempRd);
                    }

                    MergeModelsFinal();
                }
                catch (Exception ex)
                {
                    CommonTrace.WriteTrace(CommonTrace.TraceError, "Error while applying delta (id = {0}) during service initialization. {1}", delta.Id, ex.Message);
                    RestoreModel();
                }
            }

            //if (!TryApplyTransaction())
            //{
            //    RestoreModel();
            //}
        }

        private bool TryApplyTransaction()
        {
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

            bool success = false;
            if (proxyForScada.ModelUpdate(networkDataModelCopy))
                success = true;

            if (proxyForCE.ModelUpdate(networkDataModelCopy))
                success = true;

            proxyForTM.EndEnlist(success);
            try
            {
                var instance = NMSServiceBus.StartInstance().GetAwaiter().GetResult();
                var dtos = DtoConverter.Convert(networkDataModelCopy);
                var command = new ModelUpdateCommand(dtos);
                instance.Send(command).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch { }

            return success;
        }

        private void SaveDelta(Delta delta)
        {
            DeltaDto newDelta = new DeltaDto(DateTime.Now);
            newDelta.Data = delta.Serialize();
            storage.Add(newDelta);
        }

        private List<Delta> ReadAllDeltas()
        {
            List<Delta> result = new List<Delta>();
            List<DeltaDto> deltasInDB = storage.RetrieveAll("Delta");
            deltasInDB.Sort((x, y) => { return x.TimeStamp > y.TimeStamp ? 1 : -1; });
            foreach (var item in deltasInDB)
            {
                result.Add(Delta.Deserialize(item.Data));
            }
            return result;
        }

        private Dictionary<short, int> GetCounters()
        {
            Dictionary<short, int> typesCounters = new Dictionary<short, int>();

            foreach (DMSType type in Enum.GetValues(typeof(DMSType)))
            {
                typesCounters[(short)type] = 0;

                if (networkDataModelCopy.ContainsKey(type))
                {
                    typesCounters[(short)type] = GetContainer(type).Count;
                }
            }

            return typesCounters;
        }

        #region CustomMethods

        private void RestoreModel()
        {
            networkDataModelCopy = new Dictionary<DMSType, Container>();
            GetShallowCopyModel();
            SetDictionaries().GetAwaiter().GetResult();
        }
        private void MergeModelsFinal()
        {
            networkDataModel = new Dictionary<DMSType, Container>(networkDataModelCopy);
            networkDataModelCopy = new Dictionary<DMSType, Container>();
            GetShallowCopyModel();
            SetDictionaries().GetAwaiter().GetResult();
        }
        private void GetShallowCopyModel()
        {
            networkDataModelCopy = new Dictionary<DMSType, Container>();
            foreach (var item in networkDataModel)
            {
                networkDataModelCopy[item.Key] = GetContainerCopy(item.Key);
            }
        }
        private Container GetContainerCopy(DMSType type)
        {
            var container = new Container();
            container.Entities = new Dictionary<long, IdentifiedObject>();
            foreach (var item in networkDataModel[type].Entities)
            {
                container.AddEntity(item.Value);
            }
            return container;
        }

        private bool CheckMridExist(DMSType type, ResourceDescription rd)
        {
            if (networkDataModelCopy.ContainsKey(type))
            {
                var typedCollection = networkDataModelCopy[type];
                var objMrid = rd.Properties.Single(x => x.Id == ModelCode.IDOBJ_MRID);
                var result = typedCollection.Entities.Values.SingleOrDefault(x => x.MRID == objMrid.PropertyValue.StringValue);
                return result != null;
            }
            return false;

        }

        private int GetAllCounters(Dictionary<short, int> counters)
        {
            int result = 0;
            foreach (var item in counters.Values)
            {
                result += item;
            }
            return result;
        }
        #endregion

        #region ITransactionSteps
        public Task<bool> Prepare()
        {
            Console.WriteLine("NMS Prepare");
            return Task.FromResult<bool>(true);
        }

        public Task<bool> Commit()
        {
            Console.WriteLine("NMS Commit");
            MergeModelsFinal();
            return Task.FromResult<bool>(true);
        }

        public Task Rollback()
        {
            Console.WriteLine("NMS Rollback");
            RestoreModel();
            return Task.CompletedTask;
        }
        #endregion
    }
}
