using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Reflection;
using FTN.Common;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
	public enum TypeOfReference : short
	{
		Reference = 1,
		Target = 2,
		Both = 3,
	}

	public class IdentifiedObject
	{
        /// <summary>
        /// Model Resources Description
        /// </summary>
        private static ModelResourcesDesc resourcesDescs = new ModelResourcesDesc();

        private long gID;
        private string mRID;
        private string name;
        private string description;

        public IdentifiedObject(long gID)
        {
            GID = gID;
        }

        public string MRID { get => mRID; set => mRID = value; }
        public string Name { get => name; set => name = value; }
        public string Description { get => description; set => description = value; }
        public long GID { get => gID; set => gID = value; }

        public static bool operator ==(IdentifiedObject x, IdentifiedObject y)
        {
            if (Object.ReferenceEquals(x, null) && Object.ReferenceEquals(y, null))
            {
                return true;
            }
            else if ((Object.ReferenceEquals(x, null) && !Object.ReferenceEquals(y, null)) || (!Object.ReferenceEquals(x, null) && Object.ReferenceEquals(y, null)))
            {
                return false;
            }
            else
            {
                return x.Equals(y);
            }
        }

        public static bool operator !=(IdentifiedObject x, IdentifiedObject y)
        {
            return !(x == y);
        }

        public override bool Equals(object x)
        {
            if (Object.ReferenceEquals(x, null))
            {
                return false;
            }
            else
            {
                IdentifiedObject io = (IdentifiedObject)x;
                return ((io.GID == this.GID) && (io.name == this.name) && (io.mRID == this.mRID) &&
                        (io.description == this.description));
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region IAccess implementation		

        public virtual bool HasProperty(ModelCode property)
        {
            switch (property)
            {
                case ModelCode.IDOBJ_GID:
                case ModelCode.IDOBJ_NAME:
                case ModelCode.IDOBJ_DESC:
                case ModelCode.IDOBJ_MRID:
                    return true;

                default:
                    return false;
            }
        }

        public virtual void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.IDOBJ_GID:
                    property.SetValue(gID);
                    break;

                case ModelCode.IDOBJ_NAME:
                    property.SetValue(name);
                    break;

                case ModelCode.IDOBJ_MRID:
                    property.SetValue(mRID);
                    break;

                case ModelCode.IDOBJ_DESC:
                    property.SetValue(description);
                    break;

                default:
                    string message = string.Format("Unknown property id = {0} for entity (GID = 0x{1:x16}).", property.Id.ToString(), this.GID);
                    CommonTrace.WriteTrace(CommonTrace.TraceError, message);
                    throw new Exception(message);
            }
        }

        public virtual void SetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.IDOBJ_NAME:
                    name = property.AsString();
                    break;

                case ModelCode.IDOBJ_DESC:
                    description = property.AsString();
                    break;

                case ModelCode.IDOBJ_MRID:
                    mRID = property.AsString();
                    break;

                default:
                    string message = string.Format("Unknown property id = {0} for entity (GID = 0x{1:x16}).", property.Id.ToString(), this.GID);
                    CommonTrace.WriteTrace(CommonTrace.TraceError, message);
                    throw new Exception(message);
            }
        }

        #endregion IAccess implementation

        #region IReference implementation	

        public virtual bool IsReferenced
        {
            get
            {
                return false;
            }
        }


        public virtual void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            return;
        }

        public virtual void AddReference(ModelCode referenceId, long globalId)
        {
            string message = string.Format("Can not add reference {0} to entity (GID = 0x{1:x16}).", referenceId, this.GID);
            CommonTrace.WriteTrace(CommonTrace.TraceError, message);
            throw new Exception(message);
        }

        public virtual void RemoveReference(ModelCode referenceId, long globalId)
        {
            string message = string.Format("Can not remove reference {0} from entity (GID = 0x{1:x16}).", referenceId, this.GID);
            CommonTrace.WriteTrace(CommonTrace.TraceError, message);
            throw new ModelException(message);
        }

        #endregion IReference implementation

        #region utility methods

        public void GetReferences(Dictionary<ModelCode, List<long>> references)
        {
            GetReferences(references, TypeOfReference.Target | TypeOfReference.Reference);
        }

        public ResourceDescription GetAsResourceDescription(bool onlySettableAttributes)
        {
            ResourceDescription rd = new ResourceDescription(gID);
            List<ModelCode> props = new List<ModelCode>();

            if (onlySettableAttributes == true)
            {
                props = resourcesDescs.GetAllSettablePropertyIdsForEntityId(gID);
            }
            else
            {
                props = resourcesDescs.GetAllPropertyIdsForEntityId(gID);
            }

            return rd;
        }

        public ResourceDescription GetAsResourceDescription(List<ModelCode> propIds)
        {
            ResourceDescription rd = new ResourceDescription(gID);

            for (int i = 0; i < propIds.Count; i++)
            {
                rd.AddProperty(GetProperty(propIds[i]));
            }

            return rd;
        }

        public virtual Property GetProperty(ModelCode propId)
        {
            Property property = new Property(propId);
            GetProperty(property);
            return property;
        }

        public void GetDifferentProperties(IdentifiedObject compared, out List<Property> valuesInOriginal, out List<Property> valuesInCompared)
        {
            valuesInCompared = new List<Property>();
            valuesInOriginal = new List<Property>();

            ResourceDescription rd = this.GetAsResourceDescription(false);

            if (compared != null)
            {
                ResourceDescription rdCompared = compared.GetAsResourceDescription(false);

                for (int i = 0; i < rd.Properties.Count; i++)
                {
                    if (rd.Properties[i] != rdCompared.Properties[i])
                    {
                        valuesInOriginal.Add(rd.Properties[i]);
                        valuesInCompared.Add(rdCompared.Properties[i]);
                    }
                }
            }
            else
            {
                for (int i = 0; i < rd.Properties.Count; i++)
                {
                    valuesInOriginal.Add(rd.Properties[i]);
                }
            }
        }

        #endregion utility methods
    }
}
