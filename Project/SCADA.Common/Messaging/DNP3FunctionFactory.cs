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
        public static IDNP3Function CreateFunction(DNP3CommandParameters parameters)
        {
            //switch ((DNP3FunctionCode)parameters.FunctionCode)
            //{
            //    //case DNP3FunctionCode.READ:
            //    //    switch ((TypeField)parameters.ObjectTypeField)
            //    //    {
            //    //        case TypeField.CLASS_0_DATA:
            //    //            return new ReadClass0(parameters);
            //    //        default: return new Read(parameters);
            //    //    }
            //    //case DNP3FunctionCode.DIRECT_OPERATE:
            //    //    switch ((TypeField)parameters.ObjectTypeField)
            //    //    {
            //    //        case TypeField.BINARY_COMMAND:
            //    //            return new WriteDiscreteOutput(parameters);
            //    //        case TypeField.ANALOG_OUTPUT_16BIT:
            //    //            return new WriteAnalogOutput(parameters);
            //    //        default: return null;
            //    //    }
            //    //case DNP3FunctionCode.CONFIRM:
            //    //    return new Confirm(parameters);
            //    default: return null;
            //}
            return null;
        }
    }
}
