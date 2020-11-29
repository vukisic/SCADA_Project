using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using System.Collections.Generic;

namespace FTN.Services.NetworkModelService.DataModel.Wires
{
    public class RatioTapChanger : TapChanger
    {
        public long TransformerWinding { get; set; } = 0;

        public RatioTapChanger(long gID) : base(gID)
        {
        }
        public RatioTapChanger(RatioTapChanger tapChanger) : base(tapChanger)
        {
            TransformerWinding = tapChanger.TransformerWinding;
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                RatioTapChanger x = (RatioTapChanger)obj;
                return ((x.TransformerWinding == this.TransformerWinding));
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
                case ModelCode.RATIOTAPCHANGER_TRWINDING:
                    property.SetValue(TransformerWinding);
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
                case ModelCode.RATIOTAPCHANGER_TRWINDING:
                    return true;
                default:
                    return base.HasProperty(property);
            }
        }
        public override void SetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.RATIOTAPCHANGER_TRWINDING:
                    TransformerWinding = property.AsReference();
                    break;
                default:
                    base.SetProperty(property);
                    break;
            }
        }
        #endregion

        #region IReference

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (TransformerWinding != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.RATIOTAPCHANGER_TRWINDING] = new List<long>();
                references[ModelCode.RATIOTAPCHANGER_TRWINDING].Add(TransformerWinding);
            }

            base.GetReferences(references, refType);
        }

        #endregion
    }
}
