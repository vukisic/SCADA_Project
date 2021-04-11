using System.Collections.Generic;
using System.Runtime.Serialization;
using SCADA.Common.DataModel;

namespace Core.Common.ServiceBus.Events
{
    [DataContract]
    public class DomUpdateEvent
    {
        [DataMember]
        public List<SwitchingEquipment> DomData { get; set; }
    }
}
