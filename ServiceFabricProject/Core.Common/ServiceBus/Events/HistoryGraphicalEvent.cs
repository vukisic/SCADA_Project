using System.Runtime.Serialization;
using SCADA.Common.Models;

namespace Core.Common.ServiceBus.Events
{
    [DataContract]
    public class HistoryGraphicalEvent
    {
        [DataMember]
        public HistoryGraph Graph { get; set; }
    }
}
