using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common.Messaging.Parameters;

namespace SCADA.Common.Messaging
{
    public abstract class DNP3Function : IDNP3Function
    {
        private DNP3ApplicationObjectParameters commandParameters;
        private string commandOwner;

        public DNP3Function(DNP3ApplicationObjectParameters commandParameters, string commandOwner)
        {
            this.commandParameters = commandParameters;
            this.commandOwner = commandOwner;
        }

        public abstract byte[] PackRequest();

        public string CommandOwner
        {
            get
            {
                return commandOwner;
            }
            set
            {
                commandOwner = value;
            }
        }

        public DNP3ApplicationObjectParameters CommandParameters
        {
            get
            {
                return commandParameters;
            }
            set
            {
                commandParameters = value;
            }
        }
    }
}
