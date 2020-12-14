using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Common.DataModel
{
    [DataContract]
    public class AnalogPoint : BasePoint
    {
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
