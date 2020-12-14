using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Common.DataModel
{
    [DataContract]
    public enum RegisterType
    {
        [EnumMember]
        BINARY_INPUT,
        [EnumMember]
        BINARY_OUTPUT,
        [EnumMember]
        ANALOG_INPUT,
        [EnumMember]
        ANALOG_OUTPUT
    }

    [DataContract]
    public enum ClassType
    {
        [EnumMember]
        NO_CLASS,
        [EnumMember]
        CLASS_1,
        [EnumMember]
        CLASS_2,
        [EnumMember]
        CLASS_3
    }

    [DataContract]
    public enum AlarmType
    {
        [EnumMember]
        NO_ALARM,
        [EnumMember]
        ABNORMAL_VALUE,
        [EnumMember]
        HIGH_ALARM,
        [EnumMember]
        LOW_ALARM
    }
}
