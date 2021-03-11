using System.Collections.Generic;
using System.ServiceModel;
using FTN.Common;
using FTN.Services.NetworkModelService;

namespace TMContracts
{
    [ServiceContract]
    public interface IModelUpdate
    {
        [OperationContract]
        bool ModelUpdate(AffectedEntities model);
    }
}
