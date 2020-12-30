using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common.DataModel;

namespace SCADA.Services.Common
{
    [ServiceContract]
    public interface IAlarmKruncing
    {
        [OperationContract]
        List<BasePoint> checking(List<BasePoint> points);
    }
}
