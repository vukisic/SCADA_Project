using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
namespace TMContracts
{
    public class NMSCalculationEngineProxy
    {
        private IModelUpdate proxy;

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
