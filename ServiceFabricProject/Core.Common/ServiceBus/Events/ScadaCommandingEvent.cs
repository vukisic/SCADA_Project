using System.Runtime.Serialization;
using NServiceBus;
using SCADA.Common.DataModel;

namespace Core.Common.ServiceBus.Events
{
    [DataContract]
    public class ScadaCommandingEvent
    {
        [DataMember]
        public RegisterType RegisterType { get; set; }

        [DataMember]
        public uint Index { get; set; }

        [DataMember]
        public uint Value { get; set; }

        [DataMember]
        public uint Milliseconds { get; set; }
    }
}
