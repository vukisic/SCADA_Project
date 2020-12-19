using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Common.Messaging.Parameters
{
    public class DNP3ApplicationObjectParameters : DNP3FunctionParameters
    {
        /// <summary>
        /// Aplication Layer
        /// </summary>
        private byte aplicationControl;
        private byte functionCode;
        /// <summary>
        /// Object Header
        /// </summary>
        private ushort objectTypeField;
        private byte qualifier;
        private uint rangeField;
        /// <summary>
        /// Objects
        /// </summary>
        private uint prefix;
        private uint value;


        public DNP3ApplicationObjectParameters(byte aplicationControl, byte functionCode, ushort objectTypeField, byte qualifier, uint rangeField, 
            uint prefix, uint value, byte length, ushort destination, ushort source, byte transportHeader) : base(length, destination, source, transportHeader)
        {
            AplicationControl = aplicationControl;
            FunctionCode = functionCode;
            ObjectTypeField = objectTypeField;
            Qualifier = qualifier;
            RangeField = rangeField;
            Prefix = prefix;
            Value = value;
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

        public ushort ObjectTypeField
        {
            get
            {
                return objectTypeField;
            }
            set
            {
                objectTypeField = value;
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

        public uint RangeField
        {
            get
            {
                return rangeField;
            }
            set
            {
                rangeField = value;
            }
        }

        public uint Prefix
        {
            get
            {
                return prefix;
            }
            set
            {
                prefix = value;
            }
        }

        public uint Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
            }
        }
        #endregion
    }
}
