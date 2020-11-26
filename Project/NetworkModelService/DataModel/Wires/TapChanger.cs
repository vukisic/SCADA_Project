using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTN.Services.NetworkModelService.DataModel.Wires
{
    public class TapChanger : PowerSystemResource
    {
        private int highStep;
        private int lowStep;
        private int normalStep;

        public TapChanger(long gID) : base(gID)
        {
        }

        public int HighStep { get => highStep; set => highStep = value; }
        public int LowStep { get => lowStep; set => lowStep = value; }
        public int NormalStep { get => normalStep; set => normalStep = value; }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                TapChanger x = (TapChanger)obj;
                return ((x.highStep == this.highStep) && (x.lowStep == this.lowStep) && (x.normalStep == this.normalStep));
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
                case ModelCode.TAPCHANGER_HIGHSTEP:
                case ModelCode.TAPCHANGER_LOWSTEP:
                case ModelCode.TAPCHANGER_NORMALSTEP:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }
        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.TAPCHANGER_HIGHSTEP:
                    property.SetValue(highStep);
                    break;
                case ModelCode.TAPCHANGER_LOWSTEP:
                    property.SetValue(lowStep);
                    break;
                case ModelCode.TAPCHANGER_NORMALSTEP:
                    property.SetValue(normalStep);
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
                case ModelCode.TAPCHANGER_HIGHSTEP:
                    highStep = property.AsInt();
                    break;
                case ModelCode.TAPCHANGER_LOWSTEP:
                    lowStep = property.AsInt();
                    break;
                case ModelCode.TAPCHANGER_NORMALSTEP:
                    normalStep = property.AsInt();
                    break;

                default:
                    base.SetProperty(property);
                    break;
            }
        }
        #endregion
    }
}
