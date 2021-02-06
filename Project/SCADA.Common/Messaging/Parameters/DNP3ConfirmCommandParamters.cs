using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Common.Messaging.Parameters
{
    public class DNP3ConfirmCommandParamters : DNP3CommandParameters
    {
        /// <summary>
        /// Aplication Layer
        /// </summary>
        private byte aplicationControl;
        private byte functionCode;

        public DNP3ConfirmCommandParamters(byte aplicationControl, byte functionCode, byte transportControl) : base(transportControl)
        {
            AplicationControl = aplicationControl;
            FunctionCode = functionCode;
        }

        #region properties
        public byte AplicationControl { get => aplicationControl; set => aplicationControl = value; }
        public byte FunctionCode { get => functionCode; set => functionCode = value; }
        #endregion
    }
}
