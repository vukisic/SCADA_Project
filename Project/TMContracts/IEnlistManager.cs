using System.ServiceModel;

namespace TMContracts
{
    [ServiceContract]
    public interface IEnlistManager
    {
        [OperationContract]
        bool StartEnlist();

        [OperationContract]
        void Enlist();

        [OperationContract]
        void EndEnlist(bool isSuccessful);
    }
}
