using FTN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
    public class Terminal : IdentifiedObject
    {
        public Terminal(long gID) : base(gID)
        {
        }

        private long connectivityNode = 0;
        private long conductingEquipment = 0;
        private List<long> measurements = new List<long>();

        public long ConnectivityNode { get => connectivityNode; set => connectivityNode = value; }
        public long ConductingEquipment { get => conductingEquipment; set => conductingEquipment = value; }
        public List<long> Measurements { get => measurements; set => measurements = value; }


        public override bool Equals(object x)
        {
            if (base.Equals(x))
            {
                Terminal t = (Terminal)x;
                return (t.conductingEquipment == this.conductingEquipment
                        && t.connectivityNode == this.connectivityNode
                        && CompareHelper.CompareLists(t.measurements, this.measurements));
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
                case ModelCode.TERMINAL_CONDEQUIPMENT:
                case ModelCode.TERMINAL_CONNNODE:
                case ModelCode.TERMINAL_MEASUREMENTS:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.TERMINAL_CONDEQUIPMENT:
                    property.SetValue(conductingEquipment);
                    break;
                case ModelCode.TERMINAL_CONNNODE:
                    property.SetValue(connectivityNode);
                    break;
                case ModelCode.TERMINAL_MEASUREMENTS:
                    property.SetValue(measurements);
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
                case ModelCode.TERMINAL_CONDEQUIPMENT:
                    conductingEquipment = property.AsReference();
                    break;
                case ModelCode.TERMINAL_CONNNODE:
                    connectivityNode = property.AsReference();
                    break;

                default:
                    base.SetProperty(property);
                    break;
            }
        }

        #endregion

        #region IReference implementation

        public override bool IsReferenced
        {
            get
            {
                return measurements.Count != 0 || base.IsReferenced;
            }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (conductingEquipment != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.TERMINAL_CONDEQUIPMENT] = new List<long>
                {
                    conductingEquipment
                };
            }
            if (connectivityNode != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.TERMINAL_CONNNODE] = new List<long>
                {
                    connectivityNode
                };
            }
            if (measurements != null && measurements.Count != 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.TERMINAL_MEASUREMENTS] = measurements.GetRange(0, measurements.Count);
            }

            base.GetReferences(references, refType);
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.MEASUREMENT_TERMINALS:
                    measurements.Add(globalId);
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
                case ModelCode.MEASUREMENT_TERMINALS:

                    if (measurements.Contains(globalId))
                    {
                        measurements.Remove(globalId);
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
