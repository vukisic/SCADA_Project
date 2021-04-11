using System.Runtime.Serialization;

namespace Core.Common.ServiceBus.Events
{
    [DataContract]
    public class CeGraph
    {
        [DataMember]
        public CeGraphElement Pump1 { get; set; }

        [DataMember]
        public CeGraphElement Pump2 { get; set; }

        [DataMember]
        public CeGraphElement Pump3 { get; set; }

        public CeGraph()
        {
            Pump1 = new CeGraphElement();
            Pump2 = new CeGraphElement();
            Pump3 = new CeGraphElement();
        }
    }
}
