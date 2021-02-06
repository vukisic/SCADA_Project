using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Core.Common.ServiceBus.Dtos
{
    [DataContract]
    public class TerminalDto : IIdentifiedObject
    {
        [DataMember]
        public long ConductingEquipment { get; set; } = 0;
        [DataMember]
        public long ConnectivityNode { get; set; } = 0;
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
