using System.Collections.Generic;
using System.ServiceModel;
using FTN.Services.NetworkModelService;

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

        public bool ModelUpdate(AffectedEntities model)
        {
            return proxy.ModelUpdate(model);
        }
    }
}
