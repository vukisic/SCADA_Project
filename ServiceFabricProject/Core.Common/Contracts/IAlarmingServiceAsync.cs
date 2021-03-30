using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common.DataModel;

namespace Core.Common.Contracts
{
    [ServiceContract]
    public interface IAlarmingServiceAsync
    {
        [OperationContract]
        Task<Dictionary<Tuple<RegisterType, int>, BasePoint>> Check(Dictionary<Tuple<RegisterType, int>, BasePoint> points);
    }
}
