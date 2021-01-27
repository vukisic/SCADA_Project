using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Core.Common.ServiceBus.Dtos
{
    [DataContract]
    public class PowerTransformerDto : IIdentifiedObject
    {
        [DataMember]
        public List<long> TransformerWindings { get; set; } = new List<long>();
        [DataMember]
        public long EquipmentContainer { get; set; } = 0;
        [DataMember]
        public List<long> Measurements { get; set; } = new List<long>();
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public long GID { get; set; }
        [DataMember]
        public string MRID { get; set; }
        [DataMember]
        public string Name { get; set; }
    }
}
