using FTN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTN.Services.NetworkModelService.DataModel.Meas
{
    public class Analog : Measurement
    {
        public Analog(long gID) : base(gID)
        {
        }

        private float maxValue;
        private float minValue;
        private float normalValue;

        public float MaxValue { get => maxValue; set => maxValue = value; }
        public float MinValue { get => minValue; set => minValue = value; }
        public float NormalValue { get => normalValue; set => normalValue = value; }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                Analog x = (Analog)obj;
                return ((x.maxValue == this.maxValue) && (x.minValue == this.minValue) && (x.normalValue == this.normalValue));
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
        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.ANALOG_MAXVALUE:
                    property.SetValue(maxValue);
                    break;
                case ModelCode.ANALOG_MINVALUE:
                    property.SetValue(minValue);
                    break;
                case ModelCode.ANALOG_NORMALVALUE:
                    property.SetValue(normalValue);
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
                case ModelCode.ANALOG_MAXVALUE:
                    maxValue = property.AsFloat();
                    break;
                case ModelCode.ANALOG_MINVALUE:
                    minValue = property.AsFloat();
                    break;
                case ModelCode.ANALOG_NORMALVALUE:
                    normalValue = property.AsFloat();
                    break;

                default:
                    base.SetProperty(property);
                    break;
            }
        }
        #endregion
    }
}
