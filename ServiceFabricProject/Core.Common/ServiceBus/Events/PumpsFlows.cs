using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Core.Common.ServiceBus.Events
{
    [DataContract]
    public class PumpsFlows
    {
        [DataMember]
        public List<float> Flows { get; set; }

        public PumpsFlows()
        {
            Flows = new List<float>();
        }
    }
}
