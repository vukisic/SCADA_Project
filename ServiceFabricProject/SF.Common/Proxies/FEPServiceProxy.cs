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
using SCADA.Common.Models;

namespace SF.Common.Proxies
{
    public class FEPServiceProxy
    {
        private string _uri;
        public FEPServiceProxy()
        {
            _uri = "fabric:/ServiceFabricApp/FEPService";
        }

        public FEPServiceProxy(string uri)
        {
            _uri = uri;
        }

        public async Task ExecuteCommand(ScadaCommand command)
        {
            var client = BuildClient();
            await client.InvokeWithRetryAsync(x => x.Channel.ExecuteCommand(command));
        }

        private WcfClient<IFEPServiceAsync> BuildClient()
        {
            Binding binding = WcfUtility.CreateTcpClientBinding();
            IServicePartitionResolver partitionResolver = ServicePartitionResolver.GetDefault();
            var wcfClientFactory = new WcfCommunicationClientFactory<IFEPServiceAsync>(clientBinding: binding, servicePartitionResolver: partitionResolver);
            var ServiceUri = new Uri(_uri);
            var client = new WcfClient<IFEPServiceAsync>(wcfClientFactory, ServiceUri);
            return client;
        }
    }
}
