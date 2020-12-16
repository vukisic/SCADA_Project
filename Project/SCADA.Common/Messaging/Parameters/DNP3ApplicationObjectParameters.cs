using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Common.Messaging.Parameters
{
    public class DNP3ApplicationObjectParameters : DNP3FunctionParameters
    {
        private byte aplicationControl;
        private byte functionCode;
        private ushort typeField;
        private byte qualifier = 0x28;
        private uint range;
        private uint objectPrefix;
        private uint objectValue;


        public DNP3ApplicationObjectParameters(byte aplicationControl, byte functionCode, ushort typeField, byte qualifier, uint range, uint objectPrefix, uint objectValue, ushort start, byte length, byte control, ushort destination, ushort source, byte transportHeader)
                : base(start, length, control, destination, source, transportHeader)
        {
            AplicationControl = aplicationControl;
            FunctionCode = functionCode;
            TypeField = typeField;
            Qualifier = qualifier;
            Range = range;
            ObjectPrefix = objectPrefix;
            ObjectValue = objectValue;
        }

        #region properties
        public byte AplicationControl
        {
            get
            {
                return aplicationControl;
            }
            set
            {
                aplicationControl = value;
            }
        }

        public byte FunctionCode
        {
            get
            {
                return functionCode;
            }
            set
            {
                functionCode = value;
            }
        }

        public ushort TypeField
        {
            get
            {
                return typeField;
            }
            set
            {
                typeField = value;
            }
        }

        public byte Qualifier
        {
            get
            {
                return qualifier;
            }
            set
            {
                qualifier = value;
            }
        }

        public uint Range
        {
            get
            {
                return range;
            }
            set
            {
                range = value;
            }
        }

        public uint ObjectPrefix
        {
            get
            {
                return objectPrefix;
            }
            set
            {
                objectPrefix = value;
            }
        }

        public uint ObjectValue
        {
            get
            {
                return objectValue;
            }
            set
            {
                objectValue = value;
            }
        }
        #endregion
    }
}
