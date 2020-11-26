using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Wires;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
    public class AsynchronousMachine : RegulatingCondEq
    {
        public AsynchronousMachine(long gID) : base(gID)
        {
        }

        private float cosPhi;
        private float ratedP;
        private float minimumTemp;
        private float maximumTemp;
        private float currentTemp;

        public float CosPhi { get => cosPhi; set => cosPhi = value; }
        public float RatedP { get => ratedP; set => ratedP = value; }
        public float MinimumTemp { get => minimumTemp; set => minimumTemp = value; }
        public float MaximumTemp { get => maximumTemp; set => maximumTemp = value; }
        public float CurrentTemp { get => currentTemp; set => currentTemp = value; }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                AsynchronousMachine x = (AsynchronousMachine)obj;
                return ((x.cosPhi == this.cosPhi) && (x.ratedP == this.ratedP) && (x.currentTemp==this.currentTemp) && (x.minimumTemp == this.minimumTemp) && (x.maximumTemp == this.maximumTemp));
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
                case ModelCode.ASYNCMACHINE_COSPHI:
                case ModelCode.ASYNCMACHINE_RATEDP:
                case ModelCode.ASYNCMACHINE_CURRTEMP:
                case ModelCode.ASYNCMACHINE_MINTEMP:
                case ModelCode.ASYNCMACHINE_MAXTEMP:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }
        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.ASYNCMACHINE_COSPHI:
                    property.SetValue(cosPhi);
                    break;
                case ModelCode.ASYNCMACHINE_RATEDP:
                    property.SetValue(ratedP);
                    break;
                case ModelCode.ASYNCMACHINE_CURRTEMP:
                    property.SetValue(currentTemp);
                    break;
                case ModelCode.ASYNCMACHINE_MINTEMP:
                    property.SetValue(minimumTemp);
                    break;
                case ModelCode.ASYNCMACHINE_MAXTEMP:
                    property.SetValue(maximumTemp);
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
                case ModelCode.ASYNCMACHINE_COSPHI:
                    cosPhi = property.AsFloat();
                    break;
                case ModelCode.ASYNCMACHINE_RATEDP:
                    ratedP = property.AsFloat();
                    break;
                case ModelCode.ASYNCMACHINE_CURRTEMP:
                    currentTemp = property.AsFloat();
                    break;
                case ModelCode.ASYNCMACHINE_MINTEMP:
                    minimumTemp = property.AsFloat();
                    break;
                case ModelCode.ASYNCMACHINE_MAXTEMP:
                    maximumTemp = property.AsFloat();
                    break;

                default:
                    base.SetProperty(property);
                    break;
            }
        }
        #endregion
    }
}
