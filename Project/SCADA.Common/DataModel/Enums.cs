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
        CLASS_0,
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
        REASONABILITY_FAILURE,
        [EnumMember]
        ABNORMAL_VALUE,
        [EnumMember]
        HIGH_ALARM,
        [EnumMember]
        LOW_ALARM
    }

    [DataContract]
    public enum TypeField : short
    {
        [EnumMember]
        BINARY_INPUT_PACKED_FORMAT = 0x0101,
        [EnumMember]
        BINARY_INPUT_WITH_STATUS = 0x0102,
        [EnumMember]
        BINARY_OUTPUT_PACKED_FORMAT = 0x0a01,
        [EnumMember]
        BINATY_OUTPUT_WITH_STATUS = 0x0a02,
        [EnumMember]
        BINARY_COMMAND = 0x0c01,
        [EnumMember]
        ANALOG_INPUT_16BIT = 0x1e04,
        [EnumMember]
        COUNTER_16BIT = 0x1406,
        [EnumMember]
        FROZEN_COUNTER_16BIT = 0x150a,
        [EnumMember]
        ANALOG_OUTPUT_16BIT = 0x2902,
        [EnumMember]
        ANALOG_OUTPUT_STATUS_16BIT = 0x2802,
        [EnumMember]
        BINARY_OUTPUT_WITHOUT_TIME = 0x0b01,
        [EnumMember]
        BINARY_OUTPUT_EVENT = 0x0b02,
        [EnumMember]
        ANALOG_INPUT_EVENT_16BIT = 0x2002,
        [EnumMember]
        BINARY_INPUT_EVENT_WITHOUT_TIME = 0x0201,
        [EnumMember]
        ANALOG_OUTPUT_EVENT_16BIT_WITHOUT_TIME = 0x2a02,
        [EnumMember]
        FLOATING_POINT_OUTPUT_EVENT_32BIT = 0x2a07,
        [EnumMember]
        COUNTER_CHANGE_EVENT_16BIT = 0x1602,
        [EnumMember]
        CLASS_0_DATA = 0x3c01,
        [EnumMember]
        TIME_MESSAGE = 0x3201,
        [EnumMember]
        INTERNAL_INDICATIONS = 0x5001
    };

    [DataContract]
    public enum Qualifier : short
    {
        [EnumMember]
        INDEX,
        [EnumMember]
        OBJECT_SIZE,
        [EnumMember]
        START_STOP_INDEX,
        [EnumMember]
        VIRTUAL_ADDRESS,
        [EnumMember]
        NONE,
        [EnumMember]
        OBJECT_COUNT
    }

    [DataContract]
    public enum DNP3FunctionCode : short
    {
        [EnumMember]
        CONFIRM = 0,
        [EnumMember]
        READ = 1,
        [EnumMember]
        WRITE = 2,
        [EnumMember]
        SELECT = 3,
        [EnumMember]
        OPERATE = 4,
        [EnumMember]
        DIRECT_OPERATE = 5,
        [EnumMember]
        DIRECT_OPERATE_NR = 6,
        [EnumMember]
        IMMED_FREEZE = 7,
        [EnumMember]
        IMMED_FREEZE_NR = 8,
        [EnumMember]
        FREEZE_CLEAR = 9,
        [EnumMember]
        FREEZE_CLEAR_NR = 10,
        [EnumMember]
        FREEZE_AT_TIME = 11,
        [EnumMember]
        FREEZE_AT_TIME_NR = 12,
        [EnumMember]
        COLD_RESTART = 13,
        [EnumMember]
        WARM_RESTART = 14,
        [EnumMember]
        INITIALIZE_DATA = 15,
        [EnumMember]
        INITIALIZE_APPL = 16,
        [EnumMember]
        START_APPL = 17,
        [EnumMember]
        STOP_APPL = 18,
        [EnumMember]
        SAVE_CONFIG = 19,
        [EnumMember]
        ENABLE_UNSOLICITED = 20,
        [EnumMember]
        DISABLE_UNSOLICITED = 21,
        [EnumMember]
        ASSIGN_CLASS = 22,
        [EnumMember]
        DELAY_MEASUREMENT = 23,
        [EnumMember]
        RECORD_CURRENT_TIME = 24,
        [EnumMember]
        OPEN_FILE = 25,
        [EnumMember]
        CLOSE_FILE = 26,
        [EnumMember]
        DELETE_FILE = 27,
        [EnumMember]
        GET_FILE_INFO = 28,
        [EnumMember]
        AUTHENTICATE_FILE = 29,
        [EnumMember]
        ABORT_FILE = 30,
        [EnumMember]
        ACTIVATE_CONFIG = 31,
        [EnumMember]
        AUTHENTICATION_REQUEST = 32,
        [EnumMember]
        AUTHENTICATE_ERR = 33,
        [EnumMember]
        RESPONSE = 129,
        [EnumMember]
        UNSOLICITED_RESPONSE = 130,
        [EnumMember]
        AUTHENTICATE_RESP = 131
    }

    [DataContract]
    [Flags]
    public enum InternalIndications : ushort
    {
        [EnumMember]
        ALL_STATIONS = 0x0001,
        [EnumMember]
        CLASS_1_EVENTS = 0x0002,
        [EnumMember]
        CLASS_2_EVENTS = 0x0004,
        [EnumMember]
        CLASS_3_EVENTS = 0x0008,
        [EnumMember]
        NEED_TIME = 0x0010,
        [EnumMember]
        LOCAL_CONTROL = 0x0020,
        [EnumMember]
        DEVICE_TROUBLE = 0x0040,
        [EnumMember]
        DEVICE_RESTART = 0x0080,
        [EnumMember]
        NO_FUNC_CODE_SUPPORT = 0x0100,
        [EnumMember]
        OBJECT_UNKNOWN = 0x0200,
        [EnumMember]
        PARAMETER_ERROR = 0x0400,
        [EnumMember]
        EVENT_BUFFER_OVERFLOW = 0x0800,
        [EnumMember]
        ALREADY_EXECUTING = 0x1000,
        [EnumMember]
        CONFIG_CORRUPT = 0x2000,
        [EnumMember]
        RESERVED_2 = 0x4000,
        [EnumMember]
        RESERVED_1 = 0x8000,
    }
}
