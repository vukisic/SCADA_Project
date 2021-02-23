using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Common.Messaging.Parameters
{
    public class DNP3WriteCommandParameters : DNP3CommandParameters
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

        public DNP3WriteCommandParameters(byte aplicationControl, byte functionCode, ushort objectTypeField, byte qualifier, uint rangeField,
            uint prefix, uint value, byte transportHeader) : base(transportHeader)
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
        public byte AplicationControl { get => aplicationControl; set => aplicationControl = value; }
        public byte FunctionCode { get => functionCode; set => functionCode = value; }
        public ushort ObjectTypeField { get => objectTypeField; set => objectTypeField = value; }
        public byte Qualifier { get => qualifier; set => qualifier = value; }
        public uint RangeField { get => rangeField; set => rangeField = value; }
        public uint Prefix { get => prefix; set => prefix = value; }
        public uint Value { get => value; set => this.value = value; }
        #endregion
    }
}
