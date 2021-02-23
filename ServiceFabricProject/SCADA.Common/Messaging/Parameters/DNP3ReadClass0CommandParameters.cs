using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Common.Messaging.Parameters
{
    public class DNP3ReadClass0CommandParameters : DNP3CommandParameters
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

        public DNP3ReadClass0CommandParameters(byte aplicationControl, byte functionCode, ushort objectTypeField, byte qualifier, byte transportControl) : base(transportControl)
        {
            AplicationControl = aplicationControl;
            FunctionCode = functionCode;
            ObjectTypeField = objectTypeField;
            Qualifier = qualifier;
        }

        #region properties
        public byte AplicationControl { get => aplicationControl; set => aplicationControl = value; }
        public byte FunctionCode { get => functionCode; set => functionCode = value; }
        public ushort ObjectTypeField { get => objectTypeField; set => objectTypeField = value; }
        public byte Qualifier { get => qualifier; set => qualifier = value; }
        #endregion
    }
}
