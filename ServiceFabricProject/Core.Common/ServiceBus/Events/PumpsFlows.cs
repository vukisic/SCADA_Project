using System.Collections.Generic;

namespace Core.Common.ServiceBus.Events
{
    public class PumpsFlows
    {
        public List<float> Flows { get; set; }

        public PumpsFlows()
        {
            Flows = new List<float>();
        }
    }
}
