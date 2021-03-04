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
    public interface IModelUpdateAsync
    {
        [OperationContract]
        Task<bool> ModelUpdate(Dictionary<DMSType, Container> model);
    }
}
