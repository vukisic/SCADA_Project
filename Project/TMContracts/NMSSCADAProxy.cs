using System.Collections.Generic;
using System.ServiceModel;
using FTN.Services.NetworkModelService;

namespace TMContracts
{
    public class NMSSCADAProxy
    {
        private readonly IModelUpdate proxy;

        public NMSSCADAProxy()
        {
            ChannelFactory<IModelUpdate> channelFactory = new ChannelFactory<IModelUpdate>(new NetTcpBinding(), new EndpointAddress("net.tcp://localhost:5001/IModelUpdate"));
            proxy = channelFactory.CreateChannel();
        }

        public bool ModelUpdate(AffectedEntities model)
        {
            return proxy.ModelUpdate(model);
        }
    }
}
