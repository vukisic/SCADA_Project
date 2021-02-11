using System.Collections.Generic;

namespace Core.Common.ServiceBus.Events
{
    public class PumpsHours
    {
        public List<float> Hours { get; set; }
        public PumpsHours()
        {
            Hours = new List<float>();
        }
    }
}
