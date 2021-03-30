﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Contracts;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;
using SCADA.Common.DataModel;

namespace SF.Common.Proxies
{
    public class AlarmingServiceProxy
    {
        public async Task<Dictionary<Tuple<RegisterType, int>, BasePoint>> Check(Dictionary<Tuple<RegisterType, int>, BasePoint> points)
        {
            var client = BuildClient();
            return await client.InvokeWithRetryAsync(x => x.Channel.Check(points));
        }

        private WcfClient<IAlarmingService> BuildClient()
        {
            Binding binding = WcfUtility.CreateTcpClientBinding();
            IServicePartitionResolver partitionResolver = ServicePartitionResolver.GetDefault();
            var wcfClientFactory = new WcfCommunicationClientFactory<IAlarmingService>(clientBinding: binding, servicePartitionResolver: partitionResolver);
            var ServiceUri = new Uri("fabric:/ServiceFabricApp/AlarmingService");
            var client = new WcfClient<IAlarmingService>(wcfClientFactory, ServiceUri);
            return client;
        }
    }
}
