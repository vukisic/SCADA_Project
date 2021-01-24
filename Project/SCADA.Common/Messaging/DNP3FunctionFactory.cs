using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common.DataModel;
using SCADA.Common.Messaging.Messages;
using SCADA.Common.Messaging.Parameters;

namespace SCADA.Common.Messaging
{
    public class DNP3FunctionFactory
    {
        public static IDNP3Function CreateReadFunction(DNP3ReadCommandParameters parameters)
        {
            return new Read(parameters);
        }

        public static IDNP3Function CreateReadClass0Function(DNP3ReadClass0CommandParameters parameters)
        {
            return new ReadClass0(parameters);
        }

        public static IDNP3Function CreateWriteFunction(DNP3WriteCommandParameters parameters)
        {
            switch ((TypeField)parameters.ObjectTypeField)
            {
                case TypeField.BINARY_COMMAND:
                    return new WriteDiscreteOutput(parameters);
                case TypeField.ANALOG_OUTPUT_16BIT:
                    return new WriteAnalogOutput(parameters);
                default: return null;
            }
        }

        public static IDNP3Function CreateConfirmFunction(DNP3ConfirmCommandParamters parameters)
        {
            return new Confirm(parameters);
        }
    }
}
