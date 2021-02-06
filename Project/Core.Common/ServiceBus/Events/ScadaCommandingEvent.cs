using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NServiceBus;
using SCADA.Common.DataModel;

namespace Core.Common.ServiceBus.Events
{
    public class ScadaCommandingEvent : IEvent
    {
        public RegisterType RegisterType { get; set; }
        public uint Index { get; set; }
        public uint Value { get; set; }
        public uint Milliseconds { get; set; }
    }
}
