using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common.Logging;

namespace Core.Common.Contracts
{
    [ServiceContract]
    public interface ILogService
    {
        [OperationContract]
        Task Log(LogEventModel logModel);
    }
}
