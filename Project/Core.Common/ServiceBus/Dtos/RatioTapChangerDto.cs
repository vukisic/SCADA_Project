using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Core.Common.ServiceBus.Dtos
{
    [DataContract]
    public class RatioTapChangerDto : IIdentifiedObject
    {
        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public long GID { get; set; }

        [DataMember]
        public int HighStep { get; set; }

        [DataMember]
        public int LowStep { get; set; }

        [DataMember]
        public List<long> Measurements { get; set; } = new List<long>();

        [DataMember]
        public string MRID { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int NormalStep { get; set; }

        [DataMember]
        public long TransformerWinding { get; set; } = 0;
    }
}
