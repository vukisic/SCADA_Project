using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using FTN.Common;

namespace FTN.Services.NetworkModelService
{
    [DataContract]
    public class AffectedEntities
    {
        [DataMember]
        public List<long> Insert { get; set; }
        
        [DataMember]
        public List<long> Update { get; set; }
        
        [DataMember]
        public List<long> Delete { get; set; }

        public AffectedEntities()
        {
            Insert = new List<long>();
            Update = new List<long>();
            Delete = new List<long>();
        }

        public void Add(long globalId, DeltaOpType type)
        {
            if(type == DeltaOpType.Insert)
            {
                HandleInsert(globalId);
            }
            else if(type == DeltaOpType.Update)
            {
                HandleUpdate(globalId);
            }
            else
            {
                HandleDelete(globalId);
            }
        }

        private bool EntityExist(List<long> list, long globalId) => list.Contains(globalId);
        private void RemoveEntity(List<long> list, long globalId) => list.Remove(globalId);
        private void AddEntity(List<long> list, long globalId) => list.Add(globalId);

        private void HandleInsert(long globalId)
        {
            if (EntityExist(Insert, globalId))
            {
                // Do Nothing - Already exist in Insert
                return;
            }
            else if (EntityExist(Update, globalId))
            {
                // Remove from Update - Not Valid
                RemoveEntity(Update, globalId);
                return; 
            }
            else if(EntityExist(Delete, globalId))
            {
                // Remove from Delete, Add to Update
                RemoveEntity(Delete, globalId);
                AddEntity(Update, globalId);
            }
            else
            {
                // Add to Insert
                AddEntity(Insert, globalId);
            }
        }

        private void HandleUpdate(long globalId)
        {
            if (EntityExist(Insert, globalId))
            {
                // Do Nothing - Insert + Update = Insert
                return;
            }
            else if (EntityExist(Update, globalId))
            {
                // Do Nothing - Already exist in Update
                return;
            }
            else if (EntityExist(Delete, globalId))
            {
                // Remove from Delete - Not Valid
                RemoveEntity(Delete, globalId);
            }
            else
            {
                AddEntity(Update, globalId);
            }
        }

        private void HandleDelete(long globalId)
        {
            if (EntityExist(Insert, globalId))
            {
                // Remove from Insert
                RemoveEntity(Insert, globalId);
            }
            else if (EntityExist(Update, globalId))
            {
                // Remove from Update, Add to Delete
                RemoveEntity(Update, globalId);
                AddEntity(Delete, globalId);
            }
            else if (EntityExist(Delete, globalId))
            {
                // Do Nothing - Already exist in Delete
                return;
            }
            else
            {
                // Add to Delete
                AddEntity(Delete, globalId);
            }
        }
    }
}
