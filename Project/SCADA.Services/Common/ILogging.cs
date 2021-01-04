using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Services.Common
{
    [ServiceContract]
    public interface ILogging
    {
        [OperationContract]
        void Log(string level, string message);
    }
}
