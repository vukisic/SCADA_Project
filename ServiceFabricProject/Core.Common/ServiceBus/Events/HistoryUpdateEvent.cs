using System.Collections.Generic;
using System.Runtime.Serialization;
using SCADA.Common.Models;

namespace Core.Common.ServiceBus.Events
{
    [DataContract]
    public class HistoryUpdateEvent
    {
        [DataMember]
        public List<HistoryDbModel> History { get; set; }
    }
}
