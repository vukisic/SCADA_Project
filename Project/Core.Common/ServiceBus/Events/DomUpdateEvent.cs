using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NServiceBus;
using SCADA.Common.DataModel;

namespace Core.Common.ServiceBus.Events
{
    public class DomUpdateEvent : IEvent
    {
        public List<SwitchingEquipment> DomData { get; set; }
    }
}
