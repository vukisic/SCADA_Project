using FTN.Common;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
    public class Substation : EquipmentContainer
    {
        public int Capacity { get; set; }

        public Substation(long gID) : base(gID)
        {
        }

        public Substation(Substation substation) : base(substation)
        {
            Capacity = substation.Capacity;
        }

        public override bool Equals(object x)
        {
            if (base.Equals(x))
            {
                Substation s = (Substation)x;
                return this.Capacity == s.Capacity ? true : false;
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
                case ModelCode.SUBSTATION_CAPACITY:
                    property.SetValue(Capacity);
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
                case ModelCode.SUBSTATION_CAPACITY:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }
        public override void SetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.SUBSTATION_CAPACITY:
                    Capacity = property.AsInt();
                    break;
                default:
                    base.SetProperty(property);
                    break;
            }
        }

        #endregion
    }
}
