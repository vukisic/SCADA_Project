using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NServiceBus;
using SCADA.Common.Models;

namespace Core.Common.ServiceBus.Events
{
    public class HistoryUpdateEvent : IEvent
    {
        public List<HistoryDbModel> History { get; set; }
    }
}
