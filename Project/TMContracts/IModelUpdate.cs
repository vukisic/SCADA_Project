using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TMContracts
{
    [ServiceContract]
    public interface IModelUpdate
    {
        [OperationContract]
        bool UpdateModel(Dictionary<string, List<string>> model);
    }
}
