using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
