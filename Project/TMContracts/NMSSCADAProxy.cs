using System;
using System.Collections.Generic;
using System.ServiceModel;

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

        public bool ModelUpdate(Dictionary<string, List<string>> par)
        {
            return proxy.ModelUpdate(par);
        }

        public bool ModelUpdate(Dictionary<FTN.Common.DMSType, FTN.Services.NetworkModelService.Container> model)
        {
            return proxy.ModelUpdate(model);
        }
    }
}
