using FTN.Common;
using System.Collections.Generic;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
    public class Equipment : PowerSystemResource
    {
        public long EquipmentContainer { get; set; } = 0;

        public Equipment(long gID) : base(gID)
        {
        }

        public Equipment(Equipment equipment) : base(equipment)
        {
            EquipmentContainer = equipment.EquipmentContainer;
        }

        public override bool Equals(object x)
        {
            if (base.Equals(x))
            {
                Equipment e = (Equipment)x;
                return e.EquipmentContainer == this.EquipmentContainer;
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
                case ModelCode.EQUIPMENT_EQUIPCONTAINER:
                    property.SetValue(EquipmentContainer);
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
                case ModelCode.EQUIPMENT_EQUIPCONTAINER:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }
        public override void SetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.EQUIPMENT_EQUIPCONTAINER:
                    EquipmentContainer = property.AsReference();
                    break;

                default:
                    base.SetProperty(property);
                    break;
            }
        }

        #endregion

        #region IReference implementation

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (EquipmentContainer != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.EQUIPMENT_EQUIPCONTAINER] = new List<long>
                {
                    EquipmentContainer
                };
            }

            base.GetReferences(references, refType);
        }

        #endregion
    }
}
