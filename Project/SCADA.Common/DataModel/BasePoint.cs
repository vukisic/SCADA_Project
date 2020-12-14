using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using FTN.Common;

namespace SCADA.Common.DataModel
{
    [DataContract]
    [KnownType(typeof(AnalogPoint))]
    [KnownType(typeof(DiscretePoint))]
    public class BasePoint
    {
        [DataMember]
        public ClassType ClassType { get; set; }
        [DataMember]
        public SignalDirection Direction { get; set; }
        [DataMember]
        public int Index { get; set; }
        [DataMember]
        public string Mrid { get; set; }
        [DataMember]
        public string ObjectMrdi { get; set; }
        [DataMember]
        public RegisterType RegisterType { get; set; }
        [DataMember]
        public string TimeStamp { get; set; }
        [DataMember]
        public MeasurementType MeasurementType { get; set; }
        [DataMember]
        public AlarmType Alarm { get; set; }
    }
}
