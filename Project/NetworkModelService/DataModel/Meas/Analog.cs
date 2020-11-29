using FTN.Common;

namespace FTN.Services.NetworkModelService.DataModel.Meas
{
    public class Analog : Measurement
    {

        public float MaxValue { get; set; }

        public float MinValue { get; set; }

        public float NormalValue { get; set; }

        public Analog(long gID) : base(gID)
        {
        }
        public Analog(Analog analog) : base(analog)
        {
            MaxValue = analog.MaxValue;
            MinValue = analog.MinValue;
            NormalValue = analog.NormalValue;
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                Analog x = (Analog)obj;
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
                case ModelCode.ANALOG_MAXVALUE:
                    property.SetValue(MaxValue);
                    break;
                case ModelCode.ANALOG_MINVALUE:
                    property.SetValue(MinValue);
                    break;
                case ModelCode.ANALOG_NORMALVALUE:
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
                case ModelCode.ANALOG_MAXVALUE:
                case ModelCode.ANALOG_MINVALUE:
                case ModelCode.ANALOG_NORMALVALUE:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }
        public override void SetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.ANALOG_MAXVALUE:
                    MaxValue = property.AsFloat();
                    break;
                case ModelCode.ANALOG_MINVALUE:
                    MinValue = property.AsFloat();
                    break;
                case ModelCode.ANALOG_NORMALVALUE:
                    NormalValue = property.AsFloat();
                    break;

                default:
                    base.SetProperty(property);
                    break;
            }
        }
        #endregion
    }
}
