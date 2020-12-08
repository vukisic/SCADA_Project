using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TMContracts
{
    public class NMSSCADAProxy
    {
        private IModelUpdate proxy;

        public NMSSCADAProxy()
        {
            ChannelFactory<IModelUpdate> channelFactory = new ChannelFactory<IModelUpdate>(new NetTcpBinding(), new EndpointAddress("net.tcp://localhost:5001/IModeUpdate"));
            proxy = channelFactory.CreateChannel();
        }

        public bool ModelUpdate(Dictionary<string, List<string>> par)
        {
            return proxy.ModelUpdate(par);
        }
    }
}
