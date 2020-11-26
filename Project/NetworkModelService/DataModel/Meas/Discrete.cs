using FTN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTN.Services.NetworkModelService.DataModel.Meas
{
    public class Discrete : Measurement
    {
        public Discrete(long gID) : base(gID)
        {
        }

        private int maxValue;
        private int minValue;
        private int normalValue;

        public int MaxValue { get => maxValue; set => maxValue = value; }
        public int MinValue { get => minValue; set => minValue = value; }
        public int NormalValue { get => normalValue; set => normalValue = value; }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                Discrete x = (Discrete)obj;
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
                case ModelCode.DISCRETE_MAXVALUE:
                case ModelCode.DISCRETE_MINVALUE:
                case ModelCode.DISCRETE_NORMALVALUE:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }
        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.DISCRETE_MAXVALUE:
                    property.SetValue(maxValue);
                    break;
                case ModelCode.DISCRETE_MINVALUE:
                    property.SetValue(minValue);
                    break;
                case ModelCode.DISCRETE_NORMALVALUE:
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
                case ModelCode.DISCRETE_MAXVALUE:
                    maxValue = property.AsInt();
                    break;
                case ModelCode.DISCRETE_MINVALUE:
                    minValue = property.AsInt();
                    break;
                case ModelCode.DISCRETE_NORMALVALUE:
                    normalValue = property.AsInt();
                    break;

                default:
                    base.SetProperty(property);
                    break;
            }
        }
        #endregion
    }
}
