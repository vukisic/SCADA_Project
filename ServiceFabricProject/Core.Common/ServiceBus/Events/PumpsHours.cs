using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Core.Common.ServiceBus.Events
{
    [DataContract]
    public class PumpsHours
    {
        [DataMember]
        public List<float> Hours { get; set; }

        public PumpsHours()
        {
            Hours = new List<float>();
        }
    }
}
