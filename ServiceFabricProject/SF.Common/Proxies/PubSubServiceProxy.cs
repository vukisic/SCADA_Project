﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Contracts;
using Core.Common.PubSub;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;

namespace SF.Common.Proxies
{
    public class PubSubServiceProxy
    {
        public async Task SendMessage(PubSubMessage message)
        {
            var client = BuildClient();
            await client.InvokeWithRetryAsync(x => x.Channel.Publish(message));
        }

        private WcfClient<IPubSubAsync> BuildClient()
        {
            Binding binding = WcfUtility.CreateTcpClientBinding();
            IServicePartitionResolver partitionResolver = ServicePartitionResolver.GetDefault();
            var wcfClientFactory = new WcfCommunicationClientFactory<IPubSubAsync>(clientBinding: binding, servicePartitionResolver: partitionResolver);
            var ServiceUri = new Uri("fabric:/ServiceFabricApp/PubSub");
            var client = new WcfClient<IPubSubAsync>(wcfClientFactory, ServiceUri);
            return client;
        }
    }
}
