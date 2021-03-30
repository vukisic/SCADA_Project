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
using SCADA.Common.Logging;

namespace SF.Common.Proxies
{
    public class LogServiceProxy
    {
        public async Task Log(LogEventModel logModel)
        {
            var client = BuildClient();
            await client.InvokeWithRetryAsync(x => x.Channel.Log(logModel));
        }

        private WcfClient<ILogService> BuildClient()
        {
            Binding binding = WcfUtility.CreateTcpClientBinding();
            IServicePartitionResolver partitionResolver = ServicePartitionResolver.GetDefault();
            var wcfClientFactory = new WcfCommunicationClientFactory<ILogService>(clientBinding: binding, servicePartitionResolver: partitionResolver);
            var ServiceUri = new Uri("fabric:/ServiceFabricApp/LogService");
            var client = new WcfClient<ILogService>(wcfClientFactory, ServiceUri);
            return client;
        }
    }
}
