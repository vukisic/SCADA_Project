﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using FTN.Common;
using FTN.Services.NetworkModelService.DataModel;
using FTN.Services.NetworkModelService.DataModel.Core;
using FTN.Services.NetworkModelService.DataModel.Wires;
using FTN.Services.NetworkModelService.DeltaDB;

namespace FTN.Services.NetworkModelService
{	
	public class NetworkModel
	{
		/// <summary>
		/// Dictionaru which contains all data: Key - DMSType, Value - Container
		/// </summary>
		private Dictionary<DMSType, Container> networkDataModel;

        /// <summary>
        /// Dictionaru which contains all data: Key - DMSType, Value - Container => Copy
        /// </summary>
        private Dictionary<DMSType, Container> networkDataModelCopy;

        private Dictionary<long, long> GidHelper;

        /// <summary>
        /// ModelResourceDesc class contains metadata of the model
        /// </summary>
        private ModelResourcesDesc resourcesDescs;

        private IDeltaDBRepository repo;
	
		/// <summary>
		/// Initializes a new instance of the Model class.
		/// </summary>
		public NetworkModel()
		{
			networkDataModel = new Dictionary<DMSType, Container>();
            networkDataModelCopy = new Dictionary<DMSType, Container>();
			resourcesDescs = new ModelResourcesDesc();
            repo = new DeltaDBRepository();
            GidHelper = new Dictionary<long, long>();
			Initialize();
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


		/// <summary>
		/// Checks if container exists in model.
		/// </summary>
		/// <param name="type">Type of container.</param>
		/// <returns>True if container exists, otherwise FALSE.</returns>
		private bool ContainerExists(DMSType type)
		{
			if (networkDataModelCopy.ContainsKey(type))
			{
				return true;
			}
			
			return false;			
		}

		/// <summary>
		/// Gets container of specified type.
		/// </summary>
		/// <param name="type">Type of container.</param>
		/// <returns>Container for specified local id</returns>
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

		#region GDA query

		/// <summary>
		/// Gets resource description for entity requested by globalId.
		/// </summary>
		/// <param name="globalId">Id of the entity</param>
		/// <param name="properties">List of requested properties</param>		
		/// <returns>Resource description of the specified entity</returns>
		public ResourceDescription GetValues(long globalId, List<ModelCode> properties)
		{
			CommonTrace.WriteTrace(CommonTrace.TraceVerbose, String.Format("Getting values for GID = 0x{0:x16}.", globalId));

			try
			{
				IdentifiedObject io = GetEntity(globalId);

				ResourceDescription rd = new ResourceDescription(globalId);

				Property property = null;

				// insert specified properties
				foreach (ModelCode propId in properties)
				{
					property = new Property(propId);
					io.GetProperty(property);
					rd.AddProperty(property);
				}

				CommonTrace.WriteTrace(CommonTrace.TraceVerbose, String.Format("Getting values for GID = 0x{0:x16} succedded.", globalId));

				return rd;
			}			
			catch (Exception ex)
			{
				string message = string.Format("Failed to get values for entity with GID = 0x{0:x16}. {1}", globalId, ex.Message);				
				throw new Exception(message);
			}
		}

		/// <summary>
		/// Gets resource iterator that holds descriptions for all entities of the specified type.
		/// </summary>		
		/// <param name="type">Type of entity that is requested</param>
		/// <param name="properties">List of requested properties</param>		
		/// <returns>Resource iterator for the requested entities</returns>
		public ResourceIterator GetExtentValues(ModelCode entityType, List<ModelCode> properties)
		{			
			CommonTrace.WriteTrace(CommonTrace.TraceVerbose, "Getting extent values for entity type = {0} .", entityType);

			try
			{
				List<long> globalIds = new List<long>();
				Dictionary<DMSType, List<ModelCode>> class2PropertyIDs = new Dictionary<DMSType, List<ModelCode>>();
		
				DMSType entityDmsType = ModelCodeHelper.GetTypeFromModelCode(entityType);
				
				if (ContainerExists(entityDmsType))
				{
					Container container = GetContainer(entityDmsType);
					globalIds = container.GetEntitiesGlobalIds();					
					class2PropertyIDs.Add(entityDmsType, properties);					
				}

				ResourceIterator ri = new ResourceIterator(globalIds, class2PropertyIDs);

				CommonTrace.WriteTrace(CommonTrace.TraceVerbose, "Getting extent values for entity type = {0} succedded.", entityType);

				return ri;
			}		
			catch (Exception ex)
			{
				string message = string.Format("Failed to get extent values for entity type = {0}. {1}", entityType, ex.Message);				
				throw new Exception(message);
			}			
		}
		
		/// <summary>
		/// Gets resource iterator that holds descriptions for all entities related to specified source.
		/// </summary>
		/// <param name="contextId">Context Id</param>
		/// <param name="properties">List of requested properties</param>
		/// <param name="association">Relation between source and entities that should be returned</param>
		/// <param name="source">Id of entity that is start for association search</param>
		/// <param name="typeOfQuery">Query type choice(global or local)</param>
		/// <returns>Resource iterator for the requested entities</returns>
		public ResourceIterator GetRelatedValues(long source, List<ModelCode> properties, Association association)
		{
			CommonTrace.WriteTrace(CommonTrace.TraceVerbose, String.Format("Getting related values for source = 0x{0:x16}.", source));
		
			try
			{
				List<long> relatedGids = ApplyAssocioationOnSource(source, association);
			
				
				Dictionary<DMSType, List<ModelCode>> class2PropertyIDs = new Dictionary<DMSType, List<ModelCode>>();

				foreach (long relatedGid in relatedGids)
				{
					DMSType entityDmsType = (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(relatedGid);

					if (!class2PropertyIDs.ContainsKey(entityDmsType))
					{
						class2PropertyIDs.Add(entityDmsType, properties);
					}
				}

				ResourceIterator ri = new ResourceIterator(relatedGids, class2PropertyIDs);

				CommonTrace.WriteTrace(CommonTrace.TraceVerbose, String.Format("Getting related values for source = 0x{0:x16} succeeded.", source));

				return ri;
			}
			catch (Exception ex)
			{
				string message = String.Format("Failed to get related values for source GID = 0x{0:x16}. {1}.", source, ex.Message);
				CommonTrace.WriteTrace(CommonTrace.TraceError, message);
				throw new Exception(message);
			}
		}

		#endregion GDA query	

		public UpdateResult ApplyDelta(Delta delta)
		{
			bool applyingStarted = false;
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
                MergeModelsFinal();

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
				if (applyingStarted)
				{
                    if(newDelta.InsertOperations.Count>0)
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

        /// <summary>
        /// Inserts entity into the network model.
        /// </summary>
        /// <param name="rd">Description of the resource that should be inserted</param>        
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
				string message = String.Format("Failed to insert entity because entity with specified GID ({0:x16}) already exists in network model.", globalId);
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
					foreach (Property property in  rd.Properties)
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
				string message = String.Format("Failed to insert entity (GID = 0x{0:x16}) into model. {1}", rd.Id, ex.Message);				
				CommonTrace.WriteTrace(CommonTrace.TraceError, message);
				throw new Exception(message);
			}
		}


        #region Update&DeleteMethods
        /// <summary>
        /// Updates entity in block model.
        /// </summary>
        /// <param name="rd">Description of the resource that should be updated</param>		
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
						string message = String.Format("Failed to update entity because entity with specified GID ({0:x16}) does not exist in network model.", globalId);
						CommonTrace.WriteTrace(CommonTrace.TraceError, message);
						throw new Exception(message);
					}

					IdentifiedObject io = GetEntity(globalId);					

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
				string message = String.Format("Failed to update entity (GID = 0x{0:x16}) in model. {1} ", rd.Id, ex.Message);
				CommonTrace.WriteTrace(CommonTrace.TraceError, message);
				throw new Exception(message);
			}
		}

		/// <summary>
		/// Deletes resource from the netowrk model.
		/// </summary>
		/// <param name="rd">Description of the resource that should be deleted</param>		
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
					string message = String.Format("Failed to delete entity because entity with specified GID ({0:x16}) does not exist in network model.", globalId);
					CommonTrace.WriteTrace(CommonTrace.TraceError, message);
					throw new Exception(message);
				}

				// get entity to be deleted
				IdentifiedObject io = GetEntity(globalId);

				// check if entity could be deleted (if it is not referenced by any other entity)
				if (io.IsReferenced)
				{
                    Dictionary<ModelCode, List<long>> references = new Dictionary<ModelCode,List<long>>();
                    io.GetReferences(references, TypeOfReference.Target);

                    StringBuilder sb = new StringBuilder();
                    
                    foreach(KeyValuePair<ModelCode, List<long>> kvp in references)
                    {
                        foreach (long referenceGlobalId in kvp.Value)
                        {
                            sb.AppendFormat("0x{0:x16}, ", referenceGlobalId);
                        }
                    }

					string message = String.Format("Failed to delete entity (GID = 0x{0:x16}) because it is referenced by entities with GIDs: {1}.", globalId, sb.ToString());
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

				CommonTrace.WriteTrace(CommonTrace.TraceVerbose, "Deleting entity with GID ({0:x16}) successfully finished.", globalId);
			}			
			catch (Exception ex)
			{
				string message = String.Format("Failed to delete entity (GID = 0x{0:x16}) from model. {1}", rd.Id, ex.Message);
				CommonTrace.WriteTrace(CommonTrace.TraceError, message);
				throw new Exception(message);
			}
		}
        #endregion
        /// <summary>
        /// Returns related gids with source according to the association 
        /// </summary>
        /// <param name="source">source id</param>		
        /// <param name="association">desinition of association</param>
        /// <returns>related gids</returns>
        private List<long> ApplyAssocioationOnSource(long source, Association association)
		{
			List<long> relatedGids = new List<long>();

			if (association == null)
			{
				association = new Association();
			}

			IdentifiedObject io = GetEntity(source);

			if (!io.HasProperty(association.PropertyId))
			{
				throw new Exception(string.Format("Entity with GID = 0x{0:x16} does not contain prperty with Id = {1}.", source, association.PropertyId));
			}

			Property propertyRef = null;
			if (Property.GetPropertyType(association.PropertyId) == PropertyType.Reference)
			{
				propertyRef = io.GetProperty(association.PropertyId);
				long relatedGidFromProperty = propertyRef.AsReference();

				if (relatedGidFromProperty != 0)
				{
					if (association.Type == 0 || (short)ModelCodeHelper.GetTypeFromModelCode(association.Type) == ModelCodeHelper.ExtractTypeFromGlobalId(relatedGidFromProperty))
					{
						relatedGids.Add(relatedGidFromProperty);
					}
				}
			}
			else if (Property.GetPropertyType(association.PropertyId) == PropertyType.ReferenceVector)
			{
				propertyRef = io.GetProperty(association.PropertyId);
				List<long> relatedGidsFromProperty = propertyRef.AsReferences();

				if (relatedGidsFromProperty != null)
				{
					foreach (long relatedGidFromProperty in relatedGidsFromProperty)
					{
						if (association.Type == 0 || (short)ModelCodeHelper.GetTypeFromModelCode(association.Type) == ModelCodeHelper.ExtractTypeFromGlobalId(relatedGidFromProperty))
						{
							relatedGids.Add(relatedGidFromProperty);
						}
					}
				}
			}
			else
			{
				throw new Exception(string.Format("Association propertyId = {0} is not reference or reference vector type.", association.PropertyId));
			}

			return relatedGids;
		}

		/// <summary>
		/// Writes delta to log
		/// </summary>
		/// <param name="delta">delta instance which will be logged</param>
		public static void TraceDelta(Delta delta)
		{
			try
			{
				StringWriter stringWriter = new StringWriter();
				XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter);
				xmlWriter.Formatting = Formatting.Indented;
				delta.ExportToXml(xmlWriter);
				xmlWriter.Flush();
				CommonTrace.WriteTrace(CommonTrace.TraceInfo, stringWriter.ToString());				
				xmlWriter.Close();
				stringWriter.Close();
			}
			catch (Exception ex)
			{
				CommonTrace.WriteTrace(CommonTrace.TraceError, "Failed to trace delta with ID = {0}. Reason: {1}", delta.Id, ex.Message);
			}
		}

		private void Initialize()
		{
			List<Delta> result = ReadAllDeltas();
            if (result.Count > 0)
            {
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
                    }
                }
                
            }
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

        private int GetAllCounters(Dictionary<short,int> counters)
        {
            int result = 0;
            foreach (var item in counters.Values)
            {
                result += item;
            }
            return result;
        }
        #endregion
    }
}