using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Common.Messaging
{
    public interface IDNP3Function
    {
        byte[] PackRequest();
    }
}
