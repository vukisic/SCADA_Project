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
using SCADA.Common.Models;

namespace SF.Common.Proxies
{
    public class DomServiceProxy
    {
        private string _uri;
        public DomServiceProxy()
        {
            _uri = "fabric:/ServiceFabricApp/DomService";
        }

        public DomServiceProxy(string uri)
        {
            _uri = uri;
        }

        public async Task Add(List<DomDbModel> model)
        {
            var client = BuildClient();
            await client.InvokeWithRetryAsync(x => x.Channel.Add(model));
        }

        public async Task AddOrUpdate(DomDbModel model)
        {
            var client = BuildClient();
            await client.InvokeWithRetryAsync(x => x.Channel.AddOrUpdate(model));
        }

        public async Task AddOrUpdateRange(List<DomDbModel> list)
        {
            var client = BuildClient();
            await client.InvokeWithRetryAsync(x => x.Channel.AddOrUpdateRange(list));
        }

        public async Task<List<DomDbModel>> GetAll()
        {
            var client = BuildClient();
            return await client.InvokeWithRetryAsync(x => x.Channel.GetAll());
        }

        public async Task UpdateSingle(DomDbModel model)
        {
            var client = BuildClient();
            await client.InvokeWithRetryAsync(x => x.Channel.UpdateSingle(model));
        }

        private WcfClient<IDomService> BuildClient()
        {
            Binding binding = WcfUtility.CreateTcpClientBinding();
            IServicePartitionResolver partitionResolver = ServicePartitionResolver.GetDefault();
            var wcfClientFactory = new WcfCommunicationClientFactory<IDomService>(clientBinding: binding, servicePartitionResolver: partitionResolver);
            var ServiceUri = new Uri(_uri);
            var client = new WcfClient<IDomService>(wcfClientFactory, ServiceUri);
            return client;
        }
    }
}
