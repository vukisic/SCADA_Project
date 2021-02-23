using System.Runtime.Serialization;
using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Wires;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
    [DataContract]
    public class AsynchronousMachine : RegulatingCondEq
    {
        [DataMember]
        public float CosPhi { get; set; }
        [DataMember]
        public float CurrentTemp { get; set; }
        [DataMember]
        public float MaximumTemp { get; set; }
        [DataMember]
        public float MinimumTemp { get; set; }
        [DataMember]
        public float RatedP { get; set; }

        public AsynchronousMachine(long gID) : base(gID)
        {
        }
        public AsynchronousMachine(AsynchronousMachine machine) : base(machine)
        {
            CosPhi = machine.CosPhi;
            CurrentTemp = machine.CurrentTemp;
            MaximumTemp = machine.MaximumTemp;
            MinimumTemp = machine.MinimumTemp;
            RatedP = machine.RatedP;
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                AsynchronousMachine x = (AsynchronousMachine)obj;
                return ((x.CosPhi == this.CosPhi) && (x.RatedP == this.RatedP) && (x.CurrentTemp == this.CurrentTemp) && (x.MinimumTemp == this.MinimumTemp) && (x.MaximumTemp == this.MaximumTemp));
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
                case ModelCode.ASYNCMACHINE_COSPHI:
                    property.SetValue(CosPhi);
                    break;
                case ModelCode.ASYNCMACHINE_RATEDP:
                    property.SetValue(RatedP);
                    break;
                case ModelCode.ASYNCMACHINE_CURRTEMP:
                    property.SetValue(CurrentTemp);
                    break;
                case ModelCode.ASYNCMACHINE_MINTEMP:
                    property.SetValue(MinimumTemp);
                    break;
                case ModelCode.ASYNCMACHINE_MAXTEMP:
                    property.SetValue(MaximumTemp);
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
        public override void SetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.ASYNCMACHINE_COSPHI:
                    CosPhi = property.AsFloat();
                    break;
                case ModelCode.ASYNCMACHINE_RATEDP:
                    RatedP = property.AsFloat();
                    break;
                case ModelCode.ASYNCMACHINE_CURRTEMP:
                    CurrentTemp = property.AsFloat();
                    break;
                case ModelCode.ASYNCMACHINE_MINTEMP:
                    MinimumTemp = property.AsFloat();
                    break;
                case ModelCode.ASYNCMACHINE_MAXTEMP:
                    MaximumTemp = property.AsFloat();
                    break;

                default:
                    base.SetProperty(property);
                    break;
            }
        }
        #endregion
    }
}
