using FTN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
    public class ConnectivityNodeContainer : PowerSystemResource
    {
        public ConnectivityNodeContainer(long gID) : base(gID)
        {
        }

        private List<long> connectivityNodes = new List<long>();

        public List<long> ConnectivityNodes { get => connectivityNodes; set => connectivityNodes = value; }

        public override bool Equals(object x)
        {
            if (base.Equals(x))
            {
                ConnectivityNodeContainer c = (ConnectivityNodeContainer)x;
                return CompareHelper.CompareLists(c.connectivityNodes, this.connectivityNodes);
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
                case ModelCode.CONNNODECONTAINER_CONNNODES:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.CONNNODECONTAINER_CONNNODES:
                    property.SetValue(connectivityNodes);
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

        #region IReference implementation

        public override bool IsReferenced
        {
            get
            {
                return connectivityNodes.Count > 0 || base.IsReferenced;
            }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (connectivityNodes != null && connectivityNodes.Count > 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.CONNNODECONTAINER_CONNNODES] = connectivityNodes.GetRange(0, connectivityNodes.Count);
            }

            base.GetReferences(references, refType);
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.CONNECTIVITYNODE_CNODECONT:
                    connectivityNodes.Add(globalId);
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
                case ModelCode.CONNECTIVITYNODE_CNODECONT:

                    if (connectivityNodes.Contains(globalId))
                    {
                        connectivityNodes.Remove(globalId);
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
