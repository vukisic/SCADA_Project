using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Core.Common.ServiceBus.Events
{
    [DataContract]
    public class CeGraphElement
    {
        [DataMember]
        public List<DateTime> XAxes { get; set; }

        [DataMember]
        public List<long> YAxes { get; set; }

        public CeGraphElement()
        {
            XAxes = new List<DateTime>();
            YAxes = new List<long>();
        }
    }
}
