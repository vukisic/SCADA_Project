using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Contracts;
using FTN.Common;
using FTN.Services.NetworkModelService;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;

namespace SF.Common.Proxies
{
    public class CEStorageProxy
    {
        private string _uri;

        public CEStorageProxy()
        {
            _uri = "";
        }

        public CEStorageProxy(string uri)
        {
            _uri = uri;
        }
        public Task<Dictionary<DMSType, Container>> GetModel()
        {
            var client = BuildClient();
            return client.InvokeWithRetryAsync(x => x.Channel.GetModel());
        }

        public Task<Dictionary<DMSType, Container>> GetTransactionalModel()
        {
            var client = BuildClient();
            return client.InvokeWithRetryAsync(x => x.Channel.GetTransactionalModel());
        }

        public Task SetModel(Dictionary<DMSType, Container> model)
        {
            var client = BuildClient();
            return client.InvokeWithRetryAsync(x => x.Channel.SetModel(model));
        }

        public Task SetTransactionalModel(Dictionary<DMSType, Container> model)
        {
            var client = BuildClient();
            return client.InvokeWithRetryAsync(x => x.Channel.SetTransactionalModel(model));
        }

        private WcfClient<ICEStorageAsync> BuildClient()
        {
            Binding binding = WcfUtility.CreateTcpClientBinding();
            IServicePartitionResolver partitionResolver = ServicePartitionResolver.GetDefault();
            var wcfClientFactory = new WcfCommunicationClientFactory<ICEStorageAsync>(clientBinding: binding, servicePartitionResolver: partitionResolver);
            var ServiceUri = new Uri(_uri);
            var client = new WcfClient<ICEStorageAsync>(wcfClientFactory, ServiceUri, new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(1));
            return client;
        }
    }
}
