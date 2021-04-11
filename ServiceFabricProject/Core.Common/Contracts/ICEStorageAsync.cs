using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using FTN.Common;
using FTN.Services.NetworkModelService;

namespace Core.Common.Contracts
{
    [ServiceContract]
    public interface ICEStorageAsync
    {
        [OperationContract]
        Task SetModel(Dictionary<DMSType,Container> model);
        [OperationContract]
        Task SetTransactionalModel(Dictionary<DMSType, Container> model);
        [OperationContract]
        Task<Dictionary<DMSType, Container>> GetModel();
        [OperationContract]
        Task<Dictionary<DMSType, Container>> GetTransactionalModel();
    }
}
