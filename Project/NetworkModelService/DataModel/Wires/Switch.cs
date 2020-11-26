using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTN.Services.NetworkModelService.DataModel.Wires
{
    public class Switch : ConductingEquipment
    {
        private int manipulationCount;
        public int ManipulationCount { get => manipulationCount; set => manipulationCount = value; }
        public Switch(long gID) : base(gID)
        {
        }

        public override bool Equals(object x)
        {
            if (base.Equals(x))
            {
                Switch s = (Switch)x;
                return this.manipulationCount == s.ManipulationCount ? true : false;
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
                case ModelCode.SWITCH_MANIPULATIONCOUNT:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.SWITCH_MANIPULATIONCOUNT:
                    property.SetValue(manipulationCount);
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
                    manipulationCount = property.AsInt();
                    break;
                default:
                    base.SetProperty(property);
                    break;
            }
        }

        #endregion
    }
}
