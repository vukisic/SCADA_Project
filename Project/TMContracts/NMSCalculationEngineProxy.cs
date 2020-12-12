using System.Collections.Generic;
using System.ServiceModel;
namespace TMContracts
{
    public class NMSCalculationEngineProxy
    {
        private readonly IModelUpdate proxy;

        public NMSCalculationEngineProxy()
        {
            ChannelFactory<IModelUpdate> channelFactory = new ChannelFactory<IModelUpdate>(new NetTcpBinding(),
                new EndpointAddress("net.tcp://localhost:5002/IModelUpdate"));
            proxy = channelFactory.CreateChannel();
        }

        public bool ModelUpdate(Dictionary<string, List<string>> par)
        {
            return proxy.ModelUpdate(par);
        }
    }
}
