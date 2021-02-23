using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Common.Logging
{
    [DataContract]
    public class LogEventModel
    {
        [DataMember]
        public LogEventType EventType { get; set; }

        [DataMember]
        public string Message { get; set; }
    }
}
