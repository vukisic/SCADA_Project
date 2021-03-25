using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NServiceBus;
using SCADA.Common.Models;

namespace Core.Common.ServiceBus.Events
{
    public class HistoryGraphicalEvent : IEvent
    {
        public HistoryGraph Graph { get; set; }
    }
}
