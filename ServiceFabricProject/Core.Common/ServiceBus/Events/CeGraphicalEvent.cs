using System.Runtime.Serialization;

namespace Core.Common.ServiceBus.Events
{
    [DataContract]
    public class CeGraphicalEvent
    {
        [DataMember]
        public CeGraph PumpsValues { get; set; }

        public CeGraphicalEvent()
        {
            PumpsValues = new CeGraph();
        }
    }
}
