using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Core.Common.ServiceBus.Events
{
    [DataContract]
    public class CeUpdateEvent
    {
        [DataMember]
        public List<string> Times { get; set; }

        [DataMember]
        public List<double> Income { get; set; }

        [DataMember]
        public List<float> FluidLevel { get; set; }

        [DataMember]
        public List<PumpsFlows> Flows { get; set; }

        [DataMember]
        public List<PumpsHours> Hours { get; set; }
    }
}
