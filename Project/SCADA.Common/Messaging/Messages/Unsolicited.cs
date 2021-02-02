using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common.DataModel;
using SCADA.Common.Messaging.Parameters;

namespace SCADA.Common.Messaging.Messages
{
    public class Unsolicited 
    {
        public Unsolicited()
        {
        }

        public Dictionary<Tuple<RegisterType, int>, BasePoint> PareseResponse(byte[] response)
        {
            return new Dictionary<Tuple<RegisterType, int>, BasePoint>();
        }
    }
}
