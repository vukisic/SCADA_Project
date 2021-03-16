using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.ServiceBus.Events
{
    public class CeGraph
    {
        public CeGraphElement Pump1 { get; set; }
        public CeGraphElement Pump2 { get; set; }
        public CeGraphElement Pump3 { get; set; }

        public CeGraph()
        {
            Pump1 = new CeGraphElement();
            Pump2 = new CeGraphElement();
            Pump3 = new CeGraphElement();
        }
    }
}
