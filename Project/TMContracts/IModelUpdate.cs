using System.Collections.Generic;
using System.ServiceModel;

namespace TMContracts
{
    [ServiceContract]
    public interface IModelUpdate
    {
        [OperationContract]
        bool ModelUpdate(Dictionary<string, List<string>> par);
    }
}
