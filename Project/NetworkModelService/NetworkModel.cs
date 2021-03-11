using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Core.Common.ServiceBus.Commands;
using Core.Common.ServiceBus.Dtos.Conversion;
using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using FTN.Services.NetworkModelService.DeltaDB;
using NServiceBus;
using TMContracts;

namespace FTN.Services.NetworkModelService
{
    public class NetworkModel
    {
        private Dictionary<DMSType, Container> networkDataModel;
        private Dictionary<DMSType, Container> networkDataModelCopy;
        private Dictionary<long, long> GidHelper;
        private ModelResourcesDesc resourcesDescs;
        private IDeltaDBRepository repo;
        public static EventHandler<string> eventHandler;
        private AffectedEntities affectedEntities;

        public NetworkModel()
        {
            networkDataModel = new Dictionary<DMSType, Container>();
            networkDataModelCopy = new Dictionary<DMSType, Container>();
            resourcesDescs = new ModelResourcesDesc();
            repo = new DeltaDBRepository();
            GidHelper = new Dictionary<long, long>();
            eventHandler = new EventHandler<string>(HandleEvent);
            //Initialize();
        }

        private void HandleEvent(object sender, string e)
        {
            switch (e.ToLower())
            {
                case "prepare": Prepare(); break;
                case "commit": Commit(); break;
                case "rollback": Rollback(); break;
                default: break;
            }
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

        public DMSType GetDMSType(long globalId)
        {
            return (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(globalId);
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

        public IdentifiedObject GetValue(long globalId)
        {
            return GetEntity(globalId);
        }

        public List<IdentifiedObject> GetValues(List<long> globalIds)
        {
            var results = new List<IdentifiedObject>();
            foreach (var item in globalIds)
            {
                var entity = GetEntity(item);
                if (entity != null)
                    results.Add(entity);
            }
            return results;
        }

        public UpdateResult ApplyDelta(Delta delta)
        {
            bool applyingStarted = false;
            bool transactionSucceded = false;
            UpdateResult updateResult = new UpdateResult();
            Delta newDelta = new Delta();
            GetShallowCopyModel();
            affectedEntities = new AffectedEntities();
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
                #region Update&Delete
                foreach (ResourceDescription rd in delta.UpdateOperations)
                {
                    UpdateEntity(rd);
                }

                foreach (ResourceDescription rd in delta.DeleteOperations)
                {
                    DeleteEntity(rd);
                }
                #endregion

                transactionSucceded = TryApplyTransaction();

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

                // --------------------------------------------------
                affectedEntities.Add(globalId, DeltaOpType.Insert);
                // --------------------------------------------------

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

        #region Update&DeleteMethods		
        private void UpdateEntity(ResourceDescription rd)
        {
            if (rd == null || rd.Properties == null && rd.Properties.Count == 0)
            {
                CommonTrace.WriteTrace(CommonTrace.TraceInfo, "Update entity is not done because update operation is empty.");
                return;
            }

            try
            {
                long globalId = rd.Id;

                CommonTrace.WriteTrace(CommonTrace.TraceVerbose, "Updating entity with GID ({0:x16}).", globalId);

                if (!this.EntityExists(globalId))
                {
                    string message = string.Format("Failed to update entity because entity with specified GID ({0:x16}) does not exist in network model.", globalId);
                    CommonTrace.WriteTrace(CommonTrace.TraceError, message);
                    throw new Exception(message);
                }

                IdentifiedObject io = GetEntity(globalId);
                
                // --------------------------------------------------
                affectedEntities.Add(globalId, DeltaOpType.Update);
                // --------------------------------------------------

                // updating properties of entity
                foreach (Property property in rd.Properties)
                {
                    if (property.Type == PropertyType.Reference)
                    {
                        long oldTargetGlobalId = io.GetProperty(property.Id).AsReference();

                        if (oldTargetGlobalId != 0)
                        {
                            IdentifiedObject oldTargetEntity = GetEntity(oldTargetGlobalId);
                            oldTargetEntity.RemoveReference(property.Id, globalId);
                        }

                        // updating reference of entity
                        long targetGlobalId = property.AsReference();

                        if (targetGlobalId != 0)
                        {
                            if (!EntityExists(targetGlobalId))
                            {
                                string message = string.Format("Failed to get target entity with GID: 0x{0:X16}.", targetGlobalId);
                                throw new Exception(message);
                            }

                            IdentifiedObject targetEntity = GetEntity(targetGlobalId);
                            targetEntity.AddReference(property.Id, globalId);
                        }

                        // update value of the property in specified entity
                        io.SetProperty(property);
                    }
                    else
                    {
                        // update value of the property in specified entity
                        io.SetProperty(property);
                    }
                }

                CommonTrace.WriteTrace(CommonTrace.TraceVerbose, "Updating entity with GID ({0:x16}) successfully finished.", globalId);
            }
            catch (Exception ex)
            {
                string message = string.Format("Failed to update entity (GID = 0x{0:x16}) in model. {1} ", rd.Id, ex.Message);
                CommonTrace.WriteTrace(CommonTrace.TraceError, message);
                throw new Exception(message);
            }
        }

        private void DeleteEntity(ResourceDescription rd)
        {
            if (rd == null)
            {
                CommonTrace.WriteTrace(CommonTrace.TraceInfo, "Delete entity is not done because update operation is empty.");
                return;
            }

            try
            {
                long globalId = rd.Id;

                CommonTrace.WriteTrace(CommonTrace.TraceVerbose, "Deleting entity with GID ({0:x16}).", globalId);

                // check if entity exists
                if (!this.EntityExists(globalId))
                {
                    string message = string.Format("Failed to delete entity because entity with specified GID ({0:x16}) does not exist in network model.", globalId);
                    CommonTrace.WriteTrace(CommonTrace.TraceError, message);
                    throw new Exception(message);
                }

                // get entity to be deleted
                IdentifiedObject io = GetEntity(globalId);

                // check if entity could be deleted (if it is not referenced by any other entity)
                if (io.IsReferenced)
                {
                    Dictionary<ModelCode, List<long>> references = new Dictionary<ModelCode, List<long>>();
                    io.GetReferences(references, TypeOfReference.Target);

                    StringBuilder sb = new StringBuilder();

                    foreach (KeyValuePair<ModelCode, List<long>> kvp in references)
                    {
                        foreach (long referenceGlobalId in kvp.Value)
                        {
                            sb.AppendFormat("0x{0:x16}, ", referenceGlobalId);
                        }
                    }

                    string message = string.Format("Failed to delete entity (GID = 0x{0:x16}) because it is referenced by entities with GIDs: {1}.", globalId, sb.ToString());
                    CommonTrace.WriteTrace(CommonTrace.TraceError, message);
                    throw new Exception(message);
                }

                // find property ids
                List<ModelCode> propertyIds = resourcesDescs.GetAllSettablePropertyIdsForEntityId(io.GID);

                // remove references
                Property property = null;
                foreach (ModelCode propertyId in propertyIds)
                {
                    PropertyType propertyType = Property.GetPropertyType(propertyId);

                    if (propertyType == PropertyType.Reference)
                    {
                        property = io.GetProperty(propertyId);

                        if (propertyType == PropertyType.Reference)
                        {
                            // get target entity and remove reference to another entity
                            long targetGlobalId = property.AsReference();

                            if (targetGlobalId != 0)
                            {
                                // get target entity
                                IdentifiedObject targetEntity = GetEntity(targetGlobalId);

                                // remove reference to another entity
                                targetEntity.RemoveReference(propertyId, globalId);
                            }
                        }
                    }
                }

                // remove entity form netowrk model
                DMSType type = (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(globalId);
                Container container = GetContainer(type);
                container.RemoveEntity(globalId);
                
                // --------------------------------------------------
                affectedEntities.Add(globalId, DeltaOpType.Delete);
                // --------------------------------------------------
                
                CommonTrace.WriteTrace(CommonTrace.TraceVerbose, "Deleting entity with GID ({0:x16}) successfully finished.", globalId);
            }
            catch (Exception ex)
            {
                string message = string.Format("Failed to delete entity (GID = 0x{0:x16}) from model. {1}", rd.Id, ex.Message);
                CommonTrace.WriteTrace(CommonTrace.TraceError, message);
                throw new Exception(message);
            }
        }
        #endregion

        public void Initialize()
        {
            List<Delta> result = ReadAllDeltas();
            affectedEntities = new AffectedEntities();
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

                    foreach (ResourceDescription rd in delta.UpdateOperations)
                    {
                        UpdateEntity(rd);
                    }

                    foreach (ResourceDescription rd in delta.DeleteOperations)
                    {
                        DeleteEntity(rd);
                    }

                    MergeModelsFinal();
                }
                catch (Exception ex)
                {
                    CommonTrace.WriteTrace(CommonTrace.TraceError, "Error while applying delta (id = {0}) during service initialization. {1}", delta.Id, ex.Message);
                    RestoreModel();
                }
            }

            if (!TryApplyTransaction())
            {
                RestoreModel();
            }
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
            if (proxyForScada.ModelUpdate(affectedEntities))
                success = true;

            if (proxyForCE.ModelUpdate(affectedEntities))
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
            DeltaDBModel newDelta = new DeltaDBModel();
            newDelta.Data = delta.Serialize();
            repo.AddDelta(newDelta);
        }

        private List<Delta> ReadAllDeltas()
        {
            List<Delta> result = new List<Delta>();
            List<DeltaDBModel> deltasInDB = repo.GetAllDeltas();
            deltasInDB.Sort((x, y) => { return (int)(x.Id - y.Id); });
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
        }
        private void MergeModelsFinal()
        {
            networkDataModel = new Dictionary<DMSType, Container>(networkDataModelCopy);
            networkDataModelCopy = new Dictionary<DMSType, Container>();
            GetShallowCopyModel();
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

        private bool CompareResourceDescriptions(ResourceDescription rd1, ResourceDescription rd2)
        {
            foreach (var prop in rd1.Properties )
            {
                var rd2prop = rd2.GetProperty(prop.Id);
                if(rd2prop != null)
                {
                    if (!rd2prop.PropertyValue.Equals(prop.PropertyValue))
                        return true;
                }
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
        public bool Prepare()
        {
            Console.WriteLine("NMS Prepare");
            return true;
        }

        public bool Commit()
        {
            Console.WriteLine("NMS Commit");
            MergeModelsFinal();
            return true;
        }

        public void Rollback()
        {
            Console.WriteLine("NMS Rollback");
            RestoreModel();
        }
        #endregion
    }
}
