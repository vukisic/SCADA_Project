using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Common.Messaging.Parameters
{
    public class DNP3ReadCommandParameters : DNP3CommandParameters
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

        public DNP3ReadCommandParameters(byte aplicationControl, byte functionCode, ushort objectTypeField, byte qualifier, uint rangeField, byte transportControl) : base(transportControl)
        {
            AplicationControl = aplicationControl;
            FunctionCode = functionCode;
            ObjectTypeField = objectTypeField;
            Qualifier = qualifier;
            RangeField = rangeField;
        }

        #region properties
        public byte AplicationControl { get => aplicationControl; set => aplicationControl = value; }
        public byte FunctionCode { get => functionCode; set => functionCode = value; }
        public ushort ObjectTypeField { get => objectTypeField; set => objectTypeField = value; }
        public byte Qualifier { get => qualifier; set => qualifier = value; }
        public uint RangeField { get => rangeField; set => rangeField = value; }
        #endregion
    }
}
