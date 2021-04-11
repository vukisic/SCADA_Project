using System.ServiceModel;
using System.Threading.Tasks;
using Core.Common.PubSub;

namespace Core.Common.Contracts
{
    [ServiceContract]
    public interface IPubSubAsync
    {
        [OperationContract]
        Task Publish(PubSubMessage message);
    }
}
