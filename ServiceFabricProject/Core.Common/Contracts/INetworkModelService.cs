using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;

namespace Core.Common.Contracts
{
    [ServiceContract]
    public interface INetworkModelService
    {
        [OperationContract]
        Task<UpdateResult> ApplyDelta(Delta delta);

        [OperationContract]
        Task<IdentifiedObject> GetValue(long globalId);

        [OperationContract]
        Task<List<IdentifiedObject>> GetValues(List<long> globalIds);
    }
}
