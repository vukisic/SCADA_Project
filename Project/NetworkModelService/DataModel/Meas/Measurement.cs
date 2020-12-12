using System.Collections.Generic;
using System.Runtime.Serialization;
using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;

namespace FTN.Services.NetworkModelService.DataModel.Meas
{
    [DataContract]
    public class Measurement : IdentifiedObject
    {
        [DataMember]
        public int BaseAddress { get; set; }
        [DataMember]
        public SignalDirection Direction { get; set; }
        [DataMember]
        public MeasurementType MeasurementType { get; set; }
        [DataMember]
        public string ObjectMRID { get; set; }
        [DataMember]
        public long PSR { get; set; } = 0;
        [DataMember]
        public long Terminals { get; set; } = 0;
        [DataMember]
        public string TimeStamp { get; set; }

        public Measurement(long gID) : base(gID)
        {
        }
        public Measurement(Measurement measurement) : base(measurement)
        {
            BaseAddress = measurement.BaseAddress;
            Direction = measurement.Direction;
            MeasurementType = measurement.MeasurementType;
            ObjectMRID = measurement.ObjectMRID;
            PSR = measurement.PSR;
            Terminals = measurement.Terminals;
            TimeStamp = measurement.TimeStamp;
        }


        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.TERMINAL_MEASUREMENTS:
                    Terminals = globalId;
                    break;

                default:
                    base.AddReference(referenceId, globalId);
                    break;
            }
        }

        public override bool Equals(object x)
        {
            if (base.Equals(x))
            {
                Measurement m = (Measurement)x;
                return (m.Direction == this.Direction && m.MeasurementType == this.MeasurementType
                        && m.PSR == this.PSR
                        && m.ObjectMRID == this.ObjectMRID
                        && m.TimeStamp == this.TimeStamp
                        && m.BaseAddress == this.BaseAddress
                        && m.Terminals == this.Terminals);
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

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.MEASUREMENT_DIRECTION:
                    property.SetValue((short)Direction);
                    break;
                case ModelCode.MEASUREMENT_MEASTYPE:
                    property.SetValue((short)MeasurementType);
                    break;
                case ModelCode.MEASUREMENT_PSR:
                    property.SetValue(PSR);
                    break;
                case ModelCode.MEASUREMENT_TERMINAL:
                    property.SetValue(Terminals);
                    break;
                case ModelCode.MEASUREMENT_BASEADDR:
                    property.SetValue(BaseAddress);
                    break;
                case ModelCode.MEASUREMENT_OBJMRID:
                    property.SetValue(ObjectMRID);
                    break;
                case ModelCode.MEASUREMENT_TIMESTAMP:
                    property.SetValue(TimeStamp);
                    break;
                default:
                    base.GetProperty(property);
                    break;
            }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (PSR != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.MEASUREMENT_PSR] = new List<long>
                {
                    PSR
                };
            }
            if (Terminals != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.MEASUREMENT_TERMINAL] = new List<long>();
                references[ModelCode.MEASUREMENT_TERMINAL].Add(Terminals);
            }

            base.GetReferences(references, refType);
        }

        public override bool HasProperty(ModelCode property)
        {
            switch (property)
            {
                case ModelCode.MEASUREMENT_DIRECTION:
                case ModelCode.MEASUREMENT_MEASTYPE:
                case ModelCode.MEASUREMENT_PSR:
                case ModelCode.MEASUREMENT_TERMINAL:
                case ModelCode.MEASUREMENT_BASEADDR:
                case ModelCode.MEASUREMENT_OBJMRID:
                case ModelCode.MEASUREMENT_TIMESTAMP:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }
        public override void RemoveReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.TERMINAL_MEASUREMENTS:
                    if (Terminals == globalId)
                    {
                        Terminals = 0;
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

        public override void SetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.MEASUREMENT_DIRECTION:
                    Direction = (SignalDirection)property.AsEnum();
                    break;
                case ModelCode.MEASUREMENT_MEASTYPE:
                    MeasurementType = (MeasurementType)property.AsEnum();
                    break;
                case ModelCode.MEASUREMENT_PSR:
                    PSR = property.AsReference();
                    break;
                case ModelCode.MEASUREMENT_BASEADDR:
                    BaseAddress = property.AsInt();
                    break;
                case ModelCode.MEASUREMENT_OBJMRID:
                    ObjectMRID = property.AsString();
                    break;
                case ModelCode.MEASUREMENT_TIMESTAMP:
                    TimeStamp = property.AsString();
                    break;
                case ModelCode.MEASUREMENT_TERMINAL:
                    Terminals = property.AsReference();
                    break;
                default:
                    base.SetProperty(property);
                    break;
            }
        }
    }
}
