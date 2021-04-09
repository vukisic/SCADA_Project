using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Contracts;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;

namespace SF.Common.Proxies
{
    public class CEServiceProxy
    {
        public Task SetPoints(int points)
        {
            var client = BuildClient();
            return client.InvokeWithRetryAsync(x => x.Channel.SetPoints(points));
        }

        private WcfClient<ICEServiceAsync> BuildClient()
        {
            Binding binding = WcfUtility.CreateTcpClientBinding();
            IServicePartitionResolver partitionResolver = ServicePartitionResolver.GetDefault();
            var wcfClientFactory = new WcfCommunicationClientFactory<ICEServiceAsync>(clientBinding: binding, servicePartitionResolver: partitionResolver);
            var ServiceUri = new Uri("fabric:/ServiceFabricApp/CEService");
            var client = new WcfClient<ICEServiceAsync>(wcfClientFactory, ServiceUri);
            return client;
        }
    }
}
