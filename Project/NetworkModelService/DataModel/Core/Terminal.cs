using FTN.Common;
using System.Collections.Generic;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
    public class Terminal : IdentifiedObject
    {

        public long ConductingEquipment { get; set; } = 0;

        public long ConnectivityNode { get; set; } = 0;

        public List<long> Measurements { get; set; } = new List<long>();

        public Terminal(long gID) : base(gID)
        {
        }
        public Terminal(Terminal terminal) : base(terminal)
        {
            ConductingEquipment = terminal.ConductingEquipment;
            ConnectivityNode = terminal.ConnectivityNode;
            Measurements = terminal.Measurements;
        }

        public override bool Equals(object x)
        {
            if (base.Equals(x))
            {
                Terminal t = (Terminal)x;
                return (t.ConductingEquipment == this.ConductingEquipment
                        && t.ConnectivityNode == this.ConnectivityNode
                        && CompareHelper.CompareLists(t.Measurements, this.Measurements));
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

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.TERMINAL_CONDEQUIPMENT:
                    property.SetValue(ConductingEquipment);
                    break;
                case ModelCode.TERMINAL_CONNNODE:
                    property.SetValue(ConnectivityNode);
                    break;
                case ModelCode.TERMINAL_MEASUREMENTS:
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
                case ModelCode.TERMINAL_CONDEQUIPMENT:
                case ModelCode.TERMINAL_CONNNODE:
                case ModelCode.TERMINAL_MEASUREMENTS:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }
        public override void SetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.TERMINAL_CONDEQUIPMENT:
                    ConductingEquipment = property.AsReference();
                    break;
                case ModelCode.TERMINAL_CONNNODE:
                    ConnectivityNode = property.AsReference();
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
                return Measurements.Count != 0 || base.IsReferenced;
            }
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.MEASUREMENT_TERMINAL:
                    Measurements.Add(globalId);
                    break;

                default:
                    base.AddReference(referenceId, globalId);
                    break;
            }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (ConductingEquipment != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.TERMINAL_CONDEQUIPMENT] = new List<long>
                {
                    ConductingEquipment
                };
            }
            if (ConnectivityNode != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.TERMINAL_CONNNODE] = new List<long>
                {
                    ConnectivityNode
                };
            }
            if (Measurements != null && Measurements.Count != 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.TERMINAL_MEASUREMENTS] = Measurements.GetRange(0, Measurements.Count);
            }

            base.GetReferences(references, refType);
        }
        public override void RemoveReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.MEASUREMENT_TERMINAL:

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
