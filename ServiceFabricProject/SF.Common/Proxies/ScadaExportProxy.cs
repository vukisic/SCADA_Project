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
    public class ScadaExportProxy
    {
        private string _uri;

        public ScadaExportProxy()
        {
            _uri = "fabric:/ServiceFabricApp/ScadaExportService";
        }

        public ScadaExportProxy(string uri)
        {
            _uri = uri;
        }

        public async Task<Dictionary<string, BasePoint>> GetData()
        {
            var client = BuildClient();
            return await client.InvokeWithRetryAsync(x => x.Channel.GetData());
        }

        private WcfClient<IScadaExportServiceAsync> BuildClient()
        {
            Binding binding = WcfUtility.CreateTcpClientBinding();
            IServicePartitionResolver partitionResolver = ServicePartitionResolver.GetDefault();
            var wcfClientFactory = new WcfCommunicationClientFactory<IScadaExportServiceAsync>(clientBinding: binding, servicePartitionResolver: partitionResolver);
            var ServiceUri = new Uri(_uri);
            var client = new WcfClient<IScadaExportServiceAsync>(wcfClientFactory, ServiceUri);
            return client;
        }
    }
}
