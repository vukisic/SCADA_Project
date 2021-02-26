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
using SCADA.Common.DataModel;

namespace SF.Common.Proxies
{
    public class ScadaStorageProxy
    {
        public async Task<Dictionary<DMSType, Container>> GetCimModel()
        {
            var client = BuildClient();
            return await client.InvokeWithRetryAsync(x => x.Channel.GetCimModel());
        }

        public async Task<List<SwitchingEquipment>> GetDomModel()
        {
            var client = BuildClient();
            return await client.InvokeWithRetryAsync(x => x.Channel.GetDomModel());
        }

        public async Task<Dictionary<Tuple<RegisterType, int>, BasePoint>> GetModel()
        {
            var client = BuildClient();
            return await client.InvokeWithRetryAsync(x => x.Channel.GetModel());
        }

        public async Task<BasePoint> GetSingle(RegisterType type, int index)
        {
            var client = BuildClient();
            return await client.InvokeWithRetryAsync(x => x.Channel.GetSingle(type,index));
        }

        public async Task<Dictionary<Tuple<RegisterType, int>, BasePoint>> GetTransactionModel()
        {
            var client = BuildClient();
            return await client.InvokeWithRetryAsync(x => x.Channel.GetTransactionModel());
        }

        public async Task SetCimModel(Dictionary<DMSType, Container> model)
        {
            var client = BuildClient();
            await client.InvokeWithRetryAsync(x => x.Channel.SetCimModel(model));
        }

        public async Task SetDomModel(List<SwitchingEquipment> model)
        {
            var client = BuildClient();
            await client.InvokeWithRetryAsync(x => x.Channel.SetDomModel(model));
        }

        public async Task SetModel(Dictionary<Tuple<RegisterType, int>, BasePoint> model)
        {
            var client = BuildClient();
            await client.InvokeWithRetryAsync(x => x.Channel.SetModel(model));
        }

        public async Task SetTransactionModel(Dictionary<Tuple<RegisterType, int>, BasePoint> model)
        {
            var client = BuildClient();
            await client.InvokeWithRetryAsync(x => x.Channel.SetTransactionModel(model));
        }

        public async Task UpdateModelValue(Dictionary<Tuple<RegisterType, int>, BasePoint> updateModel)
        {
            var client = BuildClient();
            await client.InvokeWithRetryAsync(x => x.Channel.UpdateModelValue(updateModel));
        }

        private WcfClient<IScadaStorageService> BuildClient()
        {
            Binding binding = WcfUtility.CreateTcpClientBinding();
            IServicePartitionResolver partitionResolver = ServicePartitionResolver.GetDefault();
            var wcfClientFactory = new WcfCommunicationClientFactory<IScadaStorageService>(clientBinding: binding, servicePartitionResolver: partitionResolver);
            var ServiceUri = new Uri("fabric:/ServiceFabricApp/ScadaStorageService");
            var client = new WcfClient<IScadaStorageService>(wcfClientFactory, ServiceUri, new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(1));
            return client;
        }
    }
}
