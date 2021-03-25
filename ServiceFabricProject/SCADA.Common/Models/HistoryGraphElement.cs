using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Common.Models
{
    [DataContract]
    public class HistoryGraphElement
    {
        [DataMember]
        public List<DateTime> XAxe { get; set; }
        [DataMember]
        public List<float> YAxe { get; set; }

        public HistoryGraphElement()
        {
            XAxe = new List<DateTime>();
            YAxe = new List<float>();
        }
    }
}
