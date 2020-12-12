using FTN.Common;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
    [DataContract]
    public class EquipmentContainer : ConnectivityNodeContainer
    {
        [DataMember]
        public List<long> Equipments { get; set; } = new List<long>();

        public EquipmentContainer(long gID) : base(gID)
        {
        }
        public EquipmentContainer(EquipmentContainer container) : base(container)
        {
            Equipments = new List<long>(container.Equipments);
        }

        public override bool Equals(object x)
        {
            if (base.Equals(x))
            {
                EquipmentContainer ec = (EquipmentContainer)x;
                return CompareHelper.CompareLists(ec.Equipments, this.Equipments);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region IAccess implementation	

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.EQUIPMENTCONTAINER_EQUIPS:
                    property.SetValue(Equipments);
                    break;

                default:
                    base.GetProperty(property);
                    break;
            }
        }

        public override bool HasProperty(ModelCode property)
        {
            switch (property)
            {
                case ModelCode.EQUIPMENTCONTAINER_EQUIPS:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }
        public override void SetProperty(Property property)
        {
            base.SetProperty(property);
        }

        #endregion

        #region IReference implementation

        public override bool IsReferenced
        {
            get
            {
                return Equipments.Count != 0 || base.IsReferenced;
            }
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.EQUIPMENT_EQUIPCONTAINER:
                    Equipments.Add(globalId);
                    break;

                default:
                    base.AddReference(referenceId, globalId);
                    break;
            }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (Equipments != null && Equipments.Count != 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.EQUIPMENTCONTAINER_EQUIPS] = Equipments.GetRange(0, Equipments.Count);
            }

            base.GetReferences(references, refType);
        }
        public override void RemoveReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.EQUIPMENT_EQUIPCONTAINER:

                    if (Equipments.Contains(globalId))
                    {
                        Equipments.Remove(globalId);
                    }
                    else
                    {
                        CommonTrace.WriteTrace(CommonTrace.TraceWarning, "Entity (GID = 0x{0:x16}) doesn't contain reference 0x{1:x16}.", this.GID, globalId);
                    }

                    break;

                default:
                    base.RemoveReference(referenceId, globalId);
                    break;
            }
        }

        #endregion
    }
}
