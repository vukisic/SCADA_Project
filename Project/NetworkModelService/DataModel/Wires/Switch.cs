using System.Runtime.Serialization;
using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;

namespace FTN.Services.NetworkModelService.DataModel.Wires
{
    [DataContract]
    public class Switch : ConductingEquipment
    {
        [DataMember]
        public int ManipulationCount { get; set; }

        public Switch(long gID) : base(gID)
        {
        }
        public Switch(Switch @switch) : base(@switch)
        {
            ManipulationCount = @switch.ManipulationCount;
        }

        public override bool Equals(object x)
        {
            if (base.Equals(x))
            {
                Switch s = (Switch)x;
                return this.ManipulationCount == s.ManipulationCount ? true : false;
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
                case ModelCode.SWITCH_MANIPULATIONCOUNT:
                    property.SetValue(ManipulationCount);
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
                case ModelCode.SWITCH_MANIPULATIONCOUNT:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }
        public override void SetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.SWITCH_MANIPULATIONCOUNT:
                    ManipulationCount = property.AsInt();
                    break;
                default:
                    base.SetProperty(property);
                    break;
            }
        }

        #endregion
    }
}
