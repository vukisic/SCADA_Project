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
using SCADA.Common.DataModel;

namespace SF.Common.Proxies
{
    public class AlarmingProxy
    {
        private string _uri;
        public AlarmingProxy()
        {
            _uri = "fabric:/ServiceFabricApp/AlarmingService";
        }

        public AlarmingProxy(string uri)
        {
            _uri = uri;
        }
        public Task<Dictionary<Tuple<RegisterType, int>, BasePoint>> Check(Dictionary<Tuple<RegisterType, int>, BasePoint> points)
        {
            var client = BuildClient();
            return client.InvokeWithRetryAsync(x => x.Channel.Check(points));
        }

        private WcfClient<IAlarmingServiceAsync> BuildClient()
        {
            Binding binding = WcfUtility.CreateTcpClientBinding();
            IServicePartitionResolver partitionResolver = ServicePartitionResolver.GetDefault();
            var wcfClientFactory = new WcfCommunicationClientFactory<IAlarmingServiceAsync>(clientBinding: binding, servicePartitionResolver: partitionResolver);
            var ServiceUri = new Uri("fabric:/ServiceFabricApp/AlarmingService");
            var client = new WcfClient<IAlarmingServiceAsync>(wcfClientFactory, ServiceUri);
            return client;
        }

    }
}
