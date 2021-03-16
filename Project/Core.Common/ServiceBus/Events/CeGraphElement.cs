using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.ServiceBus.Events
{
    public class CeGraphElement
    {
        public List<DateTime> XAxes { get; set; }
        public List<long> YAxes { get; set; }

        public CeGraphElement()
        {
            XAxes = new List<DateTime>();
            YAxes = new List<long>();
        }
    }
}
