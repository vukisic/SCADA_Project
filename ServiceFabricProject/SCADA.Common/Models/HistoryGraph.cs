using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Common.Models
{
    [DataContract]
    public class HistoryGraph
    {
        [DataMember]
        public HistoryGraphElement Pump1 { get; set; }
        [DataMember]
        public HistoryGraphElement Pump2 { get; set; }
        [DataMember]
        public HistoryGraphElement Pump3 { get; set; }
        [DataMember]
        public HistoryGraphElement FluidLevel { get; set; }

        public HistoryGraph()
        {
            Pump1 = new HistoryGraphElement();
            Pump2 = new HistoryGraphElement();
            Pump3 = new HistoryGraphElement();
            FluidLevel = new HistoryGraphElement();   
        }
    }
}
