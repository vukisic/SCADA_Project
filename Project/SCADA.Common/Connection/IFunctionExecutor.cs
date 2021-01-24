using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common.Messaging;

namespace SCADA.Common.Connection
{
    public interface IFunctionExecutor
    {
        void EnqueueCommand(IDNP3Function command);
    }
}
