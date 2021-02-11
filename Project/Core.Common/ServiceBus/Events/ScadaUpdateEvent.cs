using System.Collections.Generic;
using NServiceBus;
using SCADA.Common.DataModel;

namespace Core.Common.ServiceBus.Events
{
    public class ScadaUpdateEvent : IEvent
    {
        public List<ScadaPointDto> Points { get; set; }
    }
}
