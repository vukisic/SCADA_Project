using System.Collections.Generic;
using System.Runtime.Serialization;
using NServiceBus;
using SCADA.Common.DataModel;

namespace Core.Common.ServiceBus.Events
{
    [DataContract]
    public class ScadaUpdateEvent : IEvent
    {
        [DataMember]
        public List<ScadaPointDto> Points { get; set; }
    }
}
