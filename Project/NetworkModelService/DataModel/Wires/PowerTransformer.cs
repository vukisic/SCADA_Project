using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using System.Collections.Generic;

namespace FTN.Services.NetworkModelService.DataModel.Wires
{
    public class PowerTransformer : Equipment
    {
        public List<long> TransformerWindings { get; set; } = new List<long>();

        public PowerTransformer(long gID) : base(gID)
        {
        }
        public PowerTransformer(PowerTransformer powerTransformer) : base(powerTransformer)
        {
            TransformerWindings = new List<long>(powerTransformer.TransformerWindings);
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                PowerTransformer x = (PowerTransformer)obj;
                return (CompareHelper.CompareLists(x.TransformerWindings, this.TransformerWindings));
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
                case ModelCode.POWERTRANSFORMER_TRWINDINGS:
                    property.SetValue(TransformerWindings);
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
                case ModelCode.POWERTRANSFORMER_TRWINDINGS:
                    return true;
                default:
                    return base.HasProperty(property);
            }
        }
        public override void SetProperty(Property property)
        {
            base.SetProperty(property);
        }
        #endregion

        #region IReference
        public override bool IsReferenced
        {
            get
            {
                return TransformerWindings.Count > 0 || base.IsReferenced;
            }
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.TRANSFORMERWINDING_POWERTR:
                    TransformerWindings.Add(globalId);
                    break;

                default:
                    base.AddReference(referenceId, globalId);
                    break;
            }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (TransformerWindings != null && TransformerWindings.Count != 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.POWERTRANSFORMER_TRWINDINGS] = TransformerWindings.GetRange(0, TransformerWindings.Count);
            }

            base.GetReferences(references, refType);
        }
        public override void RemoveReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.TRANSFORMERWINDING_POWERTR:

                    if (TransformerWindings.Contains(globalId))
                    {
                        TransformerWindings.Remove(globalId);
                    }
                    else
                    {
                        CommonTrace.WriteTrace(CommonTrace.TraceWarning, "Entity (GID = 0x{0:x16}) doesn't contain reference 0x{1:x16}.", this.GID, globalId);
                    }

                    break;

                default:
                    base.RemoveReference(referenceId, globalId);
                    break;
            }
        }
        #endregion
    }
}
