using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Contracts
{
    [ServiceContract]
    public interface IEnlistManagerAsync
    {
        [OperationContract]
        Task<bool> StartEnlist();

        [OperationContract]
        Task Enlist();

        [OperationContract]
        Task EndEnlist(bool isSuccessful);
    }
}
