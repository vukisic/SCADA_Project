using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTN.Services.NetworkModelService.DataModel.Meas
{
    public class Measurement : IdentifiedObject
    {
        public Measurement(long gID) : base(gID)
        {
        }

        private List<long> terminals = new List<long>();
        private long pSR = 0;
        private SignalDirection direction;
        private MeasurementType measurementType;
        private int baseAddress;
        private string objectMRID;
        private string timeStamp;

        public List<long> Terminals { get => terminals; set => terminals = value; }
        public long PSR { get => pSR; set => pSR = value; }
        public SignalDirection Direction { get => direction; set => direction = value; }
        public MeasurementType MeasurementType { get => measurementType; set => measurementType = value; }
        public int BaseAddress { get => baseAddress; set => baseAddress = value; }
        public string ObjectMRID { get => objectMRID; set => objectMRID = value; }
        public string TimeStamp { get => timeStamp; set => timeStamp = value; }

        public override bool Equals(object x)
        {
            if (base.Equals(x))
            {
                Measurement m = (Measurement)x;
                return (m.direction == this.direction && m.measurementType == this.measurementType
                        && m.pSR == this.pSR
                        && m.objectMRID == this.objectMRID
                        && m.timeStamp == this.timeStamp
                        && m.baseAddress == this.baseAddress
                        && CompareHelper.CompareLists(m.terminals, this.terminals)) ;
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

        public override bool HasProperty(ModelCode property)
        {
            switch (property)
            {
                case ModelCode.MEASUREMENT_DIRECTION:
                case ModelCode.MEASUREMENT_MEASTYPE:
                case ModelCode.MEASUREMENT_PSR:
                case ModelCode.MEASUREMENT_TERMINALS:
                case ModelCode.MEASUREMENT_BASEADDR:
                case ModelCode.MEASUREMENT_OBJMRID:
                case ModelCode.MEASUREMENT_TIMESTAMP:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.MEASUREMENT_DIRECTION:
                    property.SetValue((short)direction);
                    break;
                case ModelCode.MEASUREMENT_MEASTYPE:
                    property.SetValue((short)measurementType);
                    break;
                case ModelCode.MEASUREMENT_PSR:
                    property.SetValue(pSR);
                    break;
                case ModelCode.MEASUREMENT_TERMINALS:
                    property.SetValue(terminals);
                    break;
                case ModelCode.MEASUREMENT_BASEADDR:
                    property.SetValue(baseAddress);
                    break;
                case ModelCode.MEASUREMENT_OBJMRID:
                    property.SetValue(objectMRID);
                    break;
                case ModelCode.MEASUREMENT_TIMESTAMP:
                    property.SetValue(timeStamp);
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
                case ModelCode.MEASUREMENT_DIRECTION:
                    direction = (SignalDirection)property.AsEnum();
                    break;
                case ModelCode.MEASUREMENT_MEASTYPE:
                    measurementType = (MeasurementType)property.AsEnum();
                    break;
                case ModelCode.MEASUREMENT_PSR:
                    pSR = property.AsReference();
                    break;
                case ModelCode.MEASUREMENT_BASEADDR:
                    baseAddress = property.AsInt();
                    break;
                case ModelCode.MEASUREMENT_OBJMRID:
                    objectMRID = property.AsString();
                    break;
                case ModelCode.MEASUREMENT_TIMESTAMP:
                    timeStamp = property.AsString();
                    break;
                default:
                    base.SetProperty(property);
                    break;
            }
        }

        public override bool IsReferenced
        {
            get
            {
                return terminals.Count != 0 || base.IsReferenced;
            }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (pSR != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.MEASUREMENT_PSR] = new List<long>
                {
                    pSR
                };
            }
            if (terminals != null && terminals.Count != 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.MEASUREMENT_TERMINALS] = terminals.GetRange(0, terminals.Count);
            }

            base.GetReferences(references, refType);
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.TERMINAL_MEASUREMENTS:
                    terminals.Add(globalId);
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
                case ModelCode.TERMINAL_MEASUREMENTS:

                    if (terminals.Contains(globalId))
                    {
                        terminals.Remove(globalId);
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
    }
}
