using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Common.DataModel
{
    [DataContract]
    public class SwitchingEquipment
    {
        [DataMember]
        public string Mrid { get; set; }
        [DataMember]
        public int ManipulationConut { get; set; }
    }
}
