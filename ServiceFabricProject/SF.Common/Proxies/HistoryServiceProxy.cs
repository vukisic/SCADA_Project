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
    public class HistoryServiceProxy
    {
        public async Task Add(HistoryDbModel model)
        {
            var client = BuildClient();
            await client.InvokeWithRetryAsync(x => x.Channel.Add(model));
        }

        public async Task AddRange(List<HistoryDbModel> list)
        {
            var client = BuildClient();
            await client.InvokeWithRetryAsync(x => x.Channel.AddRange(list));
        }

        public async Task<List<HistoryDbModel>> GetAll()
        {
            var client = BuildClient();
            return await client.InvokeWithRetryAsync(x => x.Channel.GetAll());
        }

        public async Task<List<HistoryDbModel>> GetByTimestamp(DateTime timestamp)
        {
            var client = BuildClient();
            return await client.InvokeWithRetryAsync(x => x.Channel.GetByTimestamp(timestamp));
        }

        public async Task<List<HistoryDbModel>> GetInInverval(DateTime from, DateTime to)
        {
            var client = BuildClient();
            return await client.InvokeWithRetryAsync(x => x.Channel.GetInInverval(from,to));
        }

        public async Task<HistoryGraph> GetGraph()
        {
            var client = BuildClient();
            return await client.InvokeWithRetryAsync(x => x.Channel.GetGraph());
        }

        private WcfClient<IHistoryService> BuildClient()
        {
            Binding binding = WcfUtility.CreateTcpClientBinding();
            IServicePartitionResolver partitionResolver = ServicePartitionResolver.GetDefault();
            var wcfClientFactory = new WcfCommunicationClientFactory<IHistoryService>(clientBinding: binding, servicePartitionResolver: partitionResolver);
            var ServiceUri = new Uri("fabric:/ServiceFabricApp/HistoryService");
            var client = new WcfClient<IHistoryService>(wcfClientFactory, ServiceUri);
            return client;
        }
    }
}
