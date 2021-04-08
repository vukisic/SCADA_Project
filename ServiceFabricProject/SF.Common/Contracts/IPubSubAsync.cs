using Core.Common.PubSub;
using System.ServiceModel;
using System.Threading.Tasks;

namespace SF.Common.Contracts
{
    [ServiceContract]
    public interface IPubSubAsync
    {
        [OperationContract]
        Task Publish(PubSubMessage message);
    }
}
