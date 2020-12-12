using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FTN.Services.NetworkModelService.DataModel.Wires
{
    [DataContract]
    public class TransformerWinding : ConductingEquipment
    {
        [DataMember]
        public long PowerTransformer { get; set; } = 0;
        [DataMember]
        public long RatioTapChanger { get; set; } = 0;

        public TransformerWinding(long gID) : base(gID)
        {
        }
        public TransformerWinding(TransformerWinding transformerWinding) : base(transformerWinding)
        {
            PowerTransformer = transformerWinding.PowerTransformer;
            RatioTapChanger = transformerWinding.RatioTapChanger;
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                TransformerWinding x = (TransformerWinding)obj;
                return ((x.PowerTransformer == this.PowerTransformer) && (x.RatioTapChanger == this.RatioTapChanger));
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
                case ModelCode.TRANSFORMERWINDING_POWERTR:
                    property.SetValue(PowerTransformer);
                    break;
                case ModelCode.TRANSFORMERWINDING_RATIOTC:
                    property.SetValue(RatioTapChanger);
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
                case ModelCode.TRANSFORMERWINDING_POWERTR:
                case ModelCode.TRANSFORMERWINDING_RATIOTC:
                    return true;
                default:
                    return base.HasProperty(property);
            }
        }
        public override void SetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.TRANSFORMERWINDING_POWERTR:
                    PowerTransformer = property.AsReference();
                    break;
                case ModelCode.TRANSFORMERWINDING_RATIOTC:
                    RatioTapChanger = property.AsReference();
                    break;
                default:
                    base.SetProperty(property);
                    break;
            }
        }
        #endregion

        #region IReference
        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.RATIOTAPCHANGER_TRWINDING:
                    RatioTapChanger = globalId;
                    break;

                default:
                    base.AddReference(referenceId, globalId);
                    break;
            }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (PowerTransformer != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.TRANSFORMERWINDING_POWERTR] = new List<long>();
                references[ModelCode.TRANSFORMERWINDING_POWERTR].Add(PowerTransformer);
            }

            if (RatioTapChanger != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.TRANSFORMERWINDING_RATIOTC] = new List<long>();
                references[ModelCode.TRANSFORMERWINDING_RATIOTC].Add(RatioTapChanger);
            }

            base.GetReferences(references, refType);
        }
        public override void RemoveReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.RATIOTAPCHANGER_TRWINDING:

                    if (RatioTapChanger == globalId)
                    {
                        RatioTapChanger = 0;
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
