using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common.Logging;

namespace SCADA.Common.ScadaServices.Common
{
    [ServiceContract]
    public interface ILogging
    {
        [OperationContract]
        void Log(LogEventModel logModel);
    }
}
