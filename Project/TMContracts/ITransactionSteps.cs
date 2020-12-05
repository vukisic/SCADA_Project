using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TMContracts
{
    [ServiceContract]
    public interface ITransactionSteps
    {
        [OperationContract]
        bool Prepare();

        [OperationContract]
        bool Commit();

        [OperationContract]
        void Rollback();
    }
}
