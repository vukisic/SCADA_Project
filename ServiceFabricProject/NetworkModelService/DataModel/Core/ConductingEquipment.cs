using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Wires;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
    [DataContract]
    [KnownType(typeof(RegulatingCondEq))]
    [KnownType(typeof(Switch))]
    [KnownType(typeof(TransformerWinding))]
    public class ConductingEquipment : Equipment
    {
        [DataMember]
        public List<long> Terminals { get; set; } = new List<long>();

        public ConductingEquipment(long gID) : base(gID)
        {
        }
        public ConductingEquipment(ConductingEquipment equipment) : base(equipment)
        {
            Terminals = new List<long>(equipment.Terminals);
        }

        public override bool Equals(object x)
        {
            if (base.Equals(x))
            {
                ConductingEquipment c = (ConductingEquipment)x;
                return CompareHelper.CompareLists(c.Terminals, this.Terminals);
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
                case ModelCode.CONDEQ_TERMINALS:
                    property.SetValue(Terminals);
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
                case ModelCode.CONDEQ_TERMINALS:
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
                return Terminals.Count != 0 || base.IsReferenced;
            }
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.TERMINAL_CONDEQUIPMENT:
                    Terminals.Add(globalId);
                    break;

                default:
                    base.AddReference(referenceId, globalId);
                    break;
            }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (Terminals != null && Terminals.Count != 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.CONDEQ_TERMINALS] = Terminals.GetRange(0, Terminals.Count);
            }

            base.GetReferences(references, refType);
        }
        public override void RemoveReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.TERMINAL_CONDEQUIPMENT:

                    if (Terminals.Contains(globalId))
                    {
                        Terminals.Remove(globalId);
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
