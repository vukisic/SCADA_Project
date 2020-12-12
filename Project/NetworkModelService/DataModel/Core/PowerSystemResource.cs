using FTN.Common;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
    [DataContract]
    public class PowerSystemResource : IdentifiedObject
    {
        [DataMember]
        public List<long> Measurements { get; set; } = new List<long>();

        public PowerSystemResource(long gID) : base(gID)
        {
        }

        public PowerSystemResource(PowerSystemResource resource) : base(resource)
        {
            Measurements = new List<long>(resource.Measurements);
        }
        #region IAccess implementation

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.PSR_MEASUREMENTS:
                    property.SetValue(Measurements);
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
                case ModelCode.PSR_MEASUREMENTS:
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

        #region IReference implementation

        public override bool IsReferenced
        {
            get
            {
                return Measurements.Count != 0 || base.IsReferenced;
            }
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.MEASUREMENT_PSR:
                    Measurements.Add(globalId);
                    break;

                default:
                    base.AddReference(referenceId, globalId);
                    break;
            }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (Measurements != null && Measurements.Count != 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.PSR_MEASUREMENTS] = Measurements.GetRange(0, Measurements.Count);
            }

            base.GetReferences(references, refType);
        }
        public override void RemoveReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.MEASUREMENT_PSR:

                    if (Measurements.Contains(globalId))
                    {
                        Measurements.Remove(globalId);
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
