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

    public enum TypeField : short
    {
        BINARY_INPUT_PACKED_FORMAT = 0x0101,
        BINARY_INPUT_WITH_STATUS = 0x0102,
        BINARY_OUTPUT_PACKED_FORMAT = 0x0a01,
        BINATY_OUTPUT_WITH_STATUS = 0x0a02,
        BINARY_COMMAND = 0x0c01,
        ANALOG_INPUT_16BIT = 0x1e04,
        COUNTER_16BIT = 0x1406,
        FROZEN_COUNTER_16BIT = 0x150a,
        ANALOG_OUTPUT_16BIT = 0x2902,
        ANALOG_OUTPUT_STATUS_16BIT = 0x2802,
        BINARY_OUTPUT_WITHOUT_TIME = 0x0b01,
        BINARY_OUTPUT_EVENT = 0x0b02,
        ANALOG_INPUT_EVENT_16BIT = 0x2002,
        BINARY_INPUT_EVENT_WITHOUT_TIME = 0x0201,
        ANALOG_OUTPUT_EVENT_16BIT_WITHOUT_TIME = 0x2a02,
        FLOATING_POINT_OUTPUT_EVENT_32BIT = 0x2a07,
        COUNTER_CHANGE_EVENT_16BIT = 0x1602,
        CLASS_0_DATA = 0x3c01,
        TIME_MESSAGE = 0x3201,
        INTERNAL_INDICATIONS = 0x5001
    };

    public enum Qualifier : short
    {
        INDEX,
        OBJECT_SIZE,
        START_STOP_INDEX,
        VIRTUAL_ADDRESS,
        NONE,
        OBJECT_COUNT
    }

    public enum DNP3FunctionCode : short
    {
        CONFIRM = 0,
        READ = 1,
        WRITE = 2,
        SELECT = 3,
        OPERATE = 4,
        DIRECT_OPERATE = 5,
        DIRECT_OPERATE_NR = 6,
        IMMED_FREEZE = 7,
        IMMED_FREEZE_NR = 8,
        FREEZE_CLEAR = 9,
        FREEZE_CLEAR_NR = 10,
        FREEZE_AT_TIME = 11,
        FREEZE_AT_TIME_NR = 12,
        COLD_RESTART = 13,
        WARM_RESTART = 14,
        INITIALIZE_DATA = 15,
        INITIALIZE_APPL = 16,
        START_APPL = 17,
        STOP_APPL = 18,
        SAVE_CONFIG = 19,
        ENABLE_UNSOLICITED = 20,
        DISABLE_UNSOLICITED = 21,
        ASSIGN_CLASS = 22,
        DELAY_MEASUREMENT = 23,
        RECORD_CURRENT_TIME = 24,
        OPEN_FILE = 25,
        CLOSE_FILE = 26,
        DELETE_FILE = 27,
        GET_FILE_INFO = 28,
        AUTHENTICATE_FILE = 29,
        ABORT_FILE = 30,
        ACTIVATE_CONFIG = 31,
        AUTHENTICATION_REQUEST = 32,
        AUTHENTICATE_ERR = 33,
        RESPONSE = 129,
        UNSOLICITED_RESPONSE = 130,
        AUTHENTICATE_RESP = 131
    }

    [Flags]
    public enum InternalIndications : ushort
    {
        ALL_STATIONS = 0x0001,
        CLASS_1_EVENTS = 0x0002,
        CLASS_2_EVENTS = 0x0004,
        CLASS_3_EVENTS = 0x0008,
        NEED_TIME = 0x0010,
        LOCAL_CONTROL = 0x0020,
        DEVICE_TROUBLE = 0x0040,
        DEVICE_RESTART = 0x0080,
        NO_FUNC_CODE_SUPPORT = 0x0100,
        OBJECT_UNKNOWN = 0x0200,
        PARAMETER_ERROR = 0x0400,
        EVENT_BUFFER_OVERFLOW = 0x0800,
        ALREADY_EXECUTING = 0x1000,
        CONFIG_CORRUPT = 0x2000,
        RESERVED_2 = 0x4000,
        RESERVED_1 = 0x8000,
    }
}
