using System.ServiceModel;

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
