using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Core.Common.ServiceBus.Dtos
{
    [DataContract]
    public class SubstationDto : IIdentifiedObject
    {
        [DataMember]
        public int Capacity { get; set; }
        [DataMember]
        public List<long> Equipments { get; set; } = new List<long>();
        [DataMember]
        public List<long> ConnectivityNodes { get; set; } = new List<long>();
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
