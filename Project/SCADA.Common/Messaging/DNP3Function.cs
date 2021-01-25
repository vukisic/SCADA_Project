using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common.DataModel;
using SCADA.Common.Messaging.Parameters;

namespace SCADA.Common.Messaging
{
    public abstract class DNP3Function : IDNP3Function
    {
        private DNP3CommandParameters commandParameters;

        public DNP3Function(DNP3CommandParameters commandParameters)
        {
            this.commandParameters = commandParameters;
        }

        public abstract byte[] PackRequest();

        public abstract Dictionary<Tuple<RegisterType, ushort>, BasePoint> PareseResponse(byte[] response);

        public DNP3CommandParameters CommandParameters
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
