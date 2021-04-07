using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common.DataModel;

namespace SCADA.Common
{
    [DataContract]
    public class ScadaCommand
    {
        [DataMember]
        public RegisterType RegisterType { get; set; }
        [DataMember]
        public uint Index { get; set; }
        [DataMember]
        public uint Value { get; set; }
        [DataMember]
        public uint Milliseconds { get; set; }
        [DataMember]
        public bool Remove { get; set; }

        [DataMember]
        public uint MillisecondsPassedSinceLastPoll { get; set; }

        public ScadaCommand(RegisterType registerType, uint index, uint value, uint milliseconds)
        {
            RegisterType = registerType;
            Index = index;
            Value = value;
            Milliseconds = milliseconds;
            MillisecondsPassedSinceLastPoll = 0;
            Remove = false;
        }
    }
}
