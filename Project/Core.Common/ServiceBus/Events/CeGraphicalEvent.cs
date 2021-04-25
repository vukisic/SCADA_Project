using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NServiceBus;

namespace Core.Common.ServiceBus.Events
{
    public class CeGraphicalEvent : IEvent
    {
        public CeGraph PumpsValues { get; set; }

        public CeGraphicalEvent()
        {
            PumpsValues = new CeGraph();
        }
    }
}
