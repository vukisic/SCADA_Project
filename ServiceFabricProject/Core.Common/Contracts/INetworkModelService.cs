using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using FTN.Common;

namespace Core.Common.Contracts
{
    [ServiceContract]
    public interface INetworkModelService
    {
        [OperationContract]
        Task<UpdateResult> ApplyDelta(Delta delta);
    }
}
