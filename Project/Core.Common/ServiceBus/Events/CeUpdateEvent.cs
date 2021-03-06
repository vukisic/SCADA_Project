﻿using System.Collections.Generic;
using NServiceBus;

namespace Core.Common.ServiceBus.Events
{
    public class CeUpdateEvent : IEvent
    {
        public List<string> Times { get; set; }
        public List<double> Income { get; set; }
        public List<float> FluidLevel { get; set; }
        public List<PumpsFlows> Flows { get; set; }
        public List<PumpsHours> Hours { get; set; }
    }
}
