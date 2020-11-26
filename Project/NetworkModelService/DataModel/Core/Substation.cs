using FTN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
    public class Substation : EquipmentContainer
    {
        private int capacity;
        public int Capacity { get => capacity; set => capacity = value; }
        public Substation(long gID) : base(gID)
        {
        }

        public override bool Equals(object x)
        {
            if (base.Equals(x))
            {
                Substation s = (Substation)x;
                return this.capacity ==s.Capacity?true:false;
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

        public override bool HasProperty(ModelCode property)
        {
            switch (property)
            {
                case ModelCode.SUBSTATION_CAPACITY:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.SUBSTATION_CAPACITY:
                    property.SetValue(capacity);
                    break;

                default:
                    base.GetProperty(property);
                    break;
            }
        }

        public override void SetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.SUBSTATION_CAPACITY:
                    capacity = property.AsInt();
                    break;
                default:
                    base.SetProperty(property);
                    break;
            }
        }

        #endregion
    }
}
