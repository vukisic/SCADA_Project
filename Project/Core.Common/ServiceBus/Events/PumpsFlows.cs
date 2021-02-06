using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
