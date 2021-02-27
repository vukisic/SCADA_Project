using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Contracts
{
    [ServiceContract]
    public interface ITransactionStepsAsync
    {
        [OperationContract]
        Task<bool> Commit();

        [OperationContract]
        Task<bool> Prepare();

        [OperationContract]
        Task Rollback();

    }
}
