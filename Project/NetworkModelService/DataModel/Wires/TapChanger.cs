using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;

namespace FTN.Services.NetworkModelService.DataModel.Wires
{
    public class TapChanger : PowerSystemResource
    {
        public int HighStep { get; set; }

        public int LowStep { get; set; }

        public int NormalStep { get; set; }

        public TapChanger(long gID) : base(gID)
        {
        }
        public TapChanger(TapChanger tapChanger) : base(tapChanger)
        {
            HighStep = tapChanger.HighStep;
            LowStep = tapChanger.LowStep;
            NormalStep = tapChanger.NormalStep;
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                TapChanger x = (TapChanger)obj;
                return ((x.HighStep == this.HighStep) && (x.LowStep == this.LowStep) && (x.NormalStep == this.NormalStep));
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
                case ModelCode.TAPCHANGER_HIGHSTEP:
                    property.SetValue(HighStep);
                    break;
                case ModelCode.TAPCHANGER_LOWSTEP:
                    property.SetValue(LowStep);
                    break;
                case ModelCode.TAPCHANGER_NORMALSTEP:
                    property.SetValue(NormalStep);
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
                case ModelCode.TAPCHANGER_HIGHSTEP:
                case ModelCode.TAPCHANGER_LOWSTEP:
                case ModelCode.TAPCHANGER_NORMALSTEP:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }
        public override void SetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.TAPCHANGER_HIGHSTEP:
                    HighStep = property.AsInt();
                    break;
                case ModelCode.TAPCHANGER_LOWSTEP:
                    LowStep = property.AsInt();
                    break;
                case ModelCode.TAPCHANGER_NORMALSTEP:
                    NormalStep = property.AsInt();
                    break;

                default:
                    base.SetProperty(property);
                    break;
            }
        }
        #endregion
    }
}
