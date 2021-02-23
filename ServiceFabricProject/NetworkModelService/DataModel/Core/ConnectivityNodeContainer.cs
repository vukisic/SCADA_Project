using FTN.Common;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
    [DataContract]
    [KnownType(typeof(EquipmentContainer))]
    public class ConnectivityNodeContainer : PowerSystemResource
    {
        [DataMember]
        public List<long> ConnectivityNodes { get; set; } = new List<long>();

        public ConnectivityNodeContainer(long gID) : base(gID)
        {
        }
        public ConnectivityNodeContainer(ConnectivityNodeContainer container) : base(container)
        {
            ConnectivityNodes = new List<long>(container.ConnectivityNodes);
        }

        public override bool Equals(object x)
        {
            if (base.Equals(x))
            {
                ConnectivityNodeContainer c = (ConnectivityNodeContainer)x;
                return CompareHelper.CompareLists(c.ConnectivityNodes, this.ConnectivityNodes);
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
                case ModelCode.CONNNODECONTAINER_CONNNODES:
                    property.SetValue(ConnectivityNodes);
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
                case ModelCode.CONNNODECONTAINER_CONNNODES:
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
                return ConnectivityNodes.Count > 0 || base.IsReferenced;
            }
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.CONNECTIVITYNODE_CNODECONT:
                    ConnectivityNodes.Add(globalId);
                    break;

                default:
                    base.AddReference(referenceId, globalId);
                    break;
            }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (ConnectivityNodes != null && ConnectivityNodes.Count > 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.CONNNODECONTAINER_CONNNODES] = ConnectivityNodes.GetRange(0, ConnectivityNodes.Count);
            }

            base.GetReferences(references, refType);
        }
        public override void RemoveReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.CONNECTIVITYNODE_CNODECONT:

                    if (ConnectivityNodes.Contains(globalId))
                    {
                        ConnectivityNodes.Remove(globalId);
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
