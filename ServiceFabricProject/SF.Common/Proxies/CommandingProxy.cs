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
using SCADA.Common;

namespace SF.Common.Proxies
{
    public class CommandingProxy
    {
        private string _uri;
        public CommandingProxy()
        {
            _uri = "fabric:/ServiceFabricApp/CommandingService";
        }

        public CommandingProxy(string uri)
        {
            _uri = uri;
        }
        public Task Commmand(ScadaCommand command)
        {
            var client = BuildClient();
            return client.InvokeWithRetryAsync(x => x.Channel.Commmand(command));
        }

        private WcfClient<ICommandingServiceAsync> BuildClient()
        {
            Binding binding = WcfUtility.CreateTcpClientBinding();
            IServicePartitionResolver partitionResolver = ServicePartitionResolver.GetDefault();
            var wcfClientFactory = new WcfCommunicationClientFactory<ICommandingServiceAsync>(clientBinding: binding, servicePartitionResolver: partitionResolver);
            var ServiceUri = new Uri(_uri);
            var client = new WcfClient<ICommandingServiceAsync>(wcfClientFactory, ServiceUri, new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(1));
            return client;
        }
    }
}
