using System.Collections.Generic;
using NServiceBus;
using SCADA.Common.Models;

namespace Core.Common.ServiceBus.Events
{
    public class HistoryUpdateEvent : IEvent
    {
        public List<HistoryDbModel> History { get; set; }
    }
}
