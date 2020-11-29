using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using System.Collections.Generic;

namespace FTN.Services.NetworkModelService.DataModel.Topology
{
    public class ConnectivityNode : IdentifiedObject
    {
        public long ConnectivityNodeContainer { get; set; } = 0;

        public List<long> Terminals { get; set; } = new List<long>();

        public ConnectivityNode(long gID) : base(gID)
        {
        }
        public ConnectivityNode(ConnectivityNode node) : base(node)
        {
            ConnectivityNodeContainer = node.ConnectivityNodeContainer;
            Terminals = new List<long>(node.Terminals);
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                ConnectivityNode x = (ConnectivityNode)obj;
                return ((x.ConnectivityNodeContainer == this.ConnectivityNodeContainer) && CompareHelper.CompareLists(x.Terminals, this.Terminals));
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
                case ModelCode.CONNECTIVITYNODE_CNODECONT:
                    property.SetValue(ConnectivityNodeContainer);
                    break;
                case ModelCode.CONNECTIVITYNODE_TERMINALS:
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
                case ModelCode.CONNECTIVITYNODE_CNODECONT:
                case ModelCode.CONNECTIVITYNODE_TERMINALS:
                    return true;
                default:
                    return base.HasProperty(property);
            }
        }
        public override void SetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.CONNECTIVITYNODE_CNODECONT:
                    ConnectivityNodeContainer = property.AsReference();
                    break;
                default:
                    base.SetProperty(property);
                    break;
            }
        }
        #endregion

        #region IReference
        public override bool IsReferenced
        {
            get
            {
                return Terminals.Count > 0 || base.IsReferenced;
            }
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.TERMINAL_CONNNODE:
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
                references[ModelCode.CONNECTIVITYNODE_TERMINALS] = Terminals.GetRange(0, Terminals.Count);
            }

            if (ConnectivityNodeContainer != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.CONNECTIVITYNODE_CNODECONT] = new List<long>();
                references[ModelCode.CONNECTIVITYNODE_CNODECONT].Add(ConnectivityNodeContainer);
            }

            base.GetReferences(references, refType);
        }
        public override void RemoveReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.TERMINAL_CONNNODE:

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
