using FTN.Common;

namespace FTN.Services.NetworkModelService.DataModel.Meas
{
    public class Discrete : Measurement
    {
        public int MaxValue { get; set; }

        public int MinValue { get; set; }

        public int NormalValue { get; set; }

        public Discrete(long gID) : base(gID)
        {
        }
        public Discrete(Discrete discrete) : base(discrete)
        {
            MaxValue = discrete.MaxValue;
            MinValue = discrete.MinValue;
            NormalValue = discrete.NormalValue;
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                Discrete x = (Discrete)obj;
                return ((x.MaxValue == this.MaxValue) && (x.MinValue == this.MinValue) && (x.NormalValue == this.NormalValue));
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

        #region IAccess
        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.DISCRETE_MAXVALUE:
                    property.SetValue(MaxValue);
                    break;
                case ModelCode.DISCRETE_MINVALUE:
                    property.SetValue(MinValue);
                    break;
                case ModelCode.DISCRETE_NORMALVALUE:
                    property.SetValue(NormalValue);
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
                case ModelCode.DISCRETE_MAXVALUE:
                case ModelCode.DISCRETE_MINVALUE:
                case ModelCode.DISCRETE_NORMALVALUE:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }
        public override void SetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.DISCRETE_MAXVALUE:
                    MaxValue = property.AsInt();
                    break;
                case ModelCode.DISCRETE_MINVALUE:
                    MinValue = property.AsInt();
                    break;
                case ModelCode.DISCRETE_NORMALVALUE:
                    NormalValue = property.AsInt();
                    break;

                default:
                    base.SetProperty(property);
                    break;
            }
        }
        #endregion
    }
}
