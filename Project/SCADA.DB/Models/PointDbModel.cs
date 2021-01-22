using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using FTN.Common;
using SCADA.Common.DataModel;

namespace SCADA.DB.Models
{
    [DataContract]
    public class PointDbModel
    {
        [DataMember]
        public long Id { get; set; }
        [DataMember]
        public ClassType ClassType { get; set; }
        [DataMember]
        public SignalDirection Direction { get; set; }
        [DataMember]
        public int Index { get; set; }
        [DataMember]
        public string Mrid { get; set; }
        [DataMember]
        public string ObjectMrid { get; set; }
        [DataMember]
        public RegisterType RegisterType { get; set; }
        [DataMember]
        public string TimeStamp { get; set; }
        [DataMember]
        public MeasurementType MeasurementType { get; set; }
        [DataMember]
        public AlarmType Alarm { get; set; }
        [DataMember]
        public float MinValue { get; set; }
        [DataMember]
        public float MaxValue { get; set; }
        [DataMember]
        public float NormalValue { get; set; }
        [DataMember]
        public float Value { get; set; }
    }
}
