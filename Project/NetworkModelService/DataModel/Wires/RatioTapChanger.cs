using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTN.Services.NetworkModelService.DataModel.Wires
{
    public class RatioTapChanger : TapChanger
    {
        private long transformerWinding = 0;

        public RatioTapChanger(long gID) : base(gID)
        {
        }

        public long TransformerWinding { get => transformerWinding; set => transformerWinding = value; }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                RatioTapChanger x = (RatioTapChanger)obj;
                return ((x.transformerWinding == this.transformerWinding));
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
                case ModelCode.RATIOTAPCHANGER_TRWINDING:
                    return true;
                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.RATIOTAPCHANGER_TRWINDING:
                    property.SetValue(transformerWinding);
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
                case ModelCode.RATIOTAPCHANGER_TRWINDING:
                    transformerWinding = property.AsReference();
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
            if (transformerWinding != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.RATIOTAPCHANGER_TRWINDING] = new List<long>();
                references[ModelCode.RATIOTAPCHANGER_TRWINDING].Add(transformerWinding);
            }

            base.GetReferences(references, refType);
        }

        #endregion
    }
}
