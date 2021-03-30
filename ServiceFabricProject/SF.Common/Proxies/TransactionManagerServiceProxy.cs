using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Core.Common.Contracts;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;

namespace SF.Common.Proxies
{
    public class TransactionManagerServiceProxy 
    {
        public async Task<bool> StartEnlist()
        {
            var client = BuildClient();
            return await client.InvokeWithRetryAsync(x => x.Channel.StartEnlist());
        }

        public async Task Enlist()
        {
            var client = BuildClient();
            await client.InvokeWithRetryAsync(x => x.Channel.Enlist());
        }

        public async Task EndEnlist(bool isSuccessful)
        {
            var client = BuildClient();
            await client.InvokeWithRetryAsync(x => x.Channel.EndEnlist(isSuccessful));
        }

        private WcfClient<IEnlistManagerAsync> BuildClient()
        {
            Binding binding = WcfUtility.CreateTcpClientBinding();
            IServicePartitionResolver partitionResolver = ServicePartitionResolver.GetDefault();
            var wcfClientFactory = new WcfCommunicationClientFactory<IEnlistManagerAsync>(clientBinding: binding, servicePartitionResolver: partitionResolver);
            var ServiceUri = new Uri("fabric:/ServiceFabricApp/TransactionManagerService");
            var client = new WcfClient<IEnlistManagerAsync>(wcfClientFactory, ServiceUri, new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(1));
            return client;
        }
    }
}
