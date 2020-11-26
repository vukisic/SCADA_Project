using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;


namespace FTN.Services.NetworkModelService.DataModel.Wires
{
    public class PowerTransformer : Equipment
    {
        private List<long> transformerWindings = new List<long>();

        public PowerTransformer(long gID) : base(gID)
        {
        }

        public List<long> TransformerWindings { get => transformerWindings; set => transformerWindings = value; }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                PowerTransformer x = (PowerTransformer)obj;
                return (CompareHelper.CompareLists(x.transformerWindings, this.transformerWindings));
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
                case ModelCode.POWERTRANSFORMER_TRWINDINGS:
                    return true;
                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.POWERTRANSFORMER_TRWINDINGS:
                    property.SetValue(transformerWindings);
                    break;
                default:
                    base.GetProperty(property);
                    break;
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
                return transformerWindings.Count > 0 || base.IsReferenced;
            }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (transformerWindings != null && transformerWindings.Count != 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.POWERTRANSFORMER_TRWINDINGS] = transformerWindings.GetRange(0, transformerWindings.Count);
            }

            base.GetReferences(references, refType);
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.TRANSFORMERWINDING_POWERTR:
                    transformerWindings.Add(globalId);
                    break;

                default:
                    base.AddReference(referenceId, globalId);
                    break;
            }
        }

        public override void RemoveReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.TRANSFORMERWINDING_POWERTR:

                    if (transformerWindings.Contains(globalId))
                    {
                        transformerWindings.Remove(globalId);
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
