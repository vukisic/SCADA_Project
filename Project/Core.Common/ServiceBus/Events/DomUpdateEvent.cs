using System.Collections.Generic;
using NServiceBus;
using SCADA.Common.DataModel;

namespace Core.Common.ServiceBus.Events
{
    public class DomUpdateEvent : IEvent
    {
        public List<SwitchingEquipment> DomData { get; set; }
    }
}
