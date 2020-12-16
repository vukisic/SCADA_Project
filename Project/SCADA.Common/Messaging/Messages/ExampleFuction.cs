using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common.Messaging.Parameters;

namespace SCADA.Common.Messaging.Messages
{
    // TO DO: Delete this
    public class ExampleFuction : DNP3Function
    {
        public ExampleFuction(DNP3ApplicationObjectParameters commandParameters) : base(commandParameters)
        {

        }

        public override byte[] PackRequest()
        {
            return null;
        }
    }
}
