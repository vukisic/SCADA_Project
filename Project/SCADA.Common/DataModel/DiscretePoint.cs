using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Common.DataModel
{
    [DataContract]
    public class DiscretePoint : BasePoint
    {
        [DataMember]
        public int MinValue { get; set; }
        [DataMember]
        public int MaxValue { get; set; }
        [DataMember]
        public int NormalValue { get; set; }
        [DataMember]
        public int Value { get; set; }
    }
}
