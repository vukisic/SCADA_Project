using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common.DataModel;

namespace SCADA.Common.ScadaServices.Common
{
    [ServiceContract]
    public interface IScadaExport
    {
        [OperationContract]
        Dictionary<string, BasePoint> GetData();
    }
}
