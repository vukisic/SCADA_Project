using Core.Common.Contracts;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Fabric;
using System.Fabric.Description;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkModelServiceSF
{
    internal sealed class NetworkModelServiceSF : StatefulService
    {
        public NetworkModelServiceSF(StatefulServiceContext context)
            : base(context)
        { }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[] { new ServiceReplicaListener((context) =>
                {
                    string host = host = context.NodeContext.IPAddressOrFQDN;

                    EndpointResourceDescription endpointConfig = context.CodePackageActivationContext.GetEndpoint("ServiceEndpoint");
                    int port = endpointConfig.Port;
                    string scheme = endpointConfig.Protocol.ToString();
                    string uri = string.Format(CultureInfo.InvariantCulture, "{0}://{1}:{2}/NetworkModelServiceSF", "net.tcp", host, port);
                    var listener = new WcfCommunicationListener<INetworkModelService>(
                        wcfServiceObject: new NetworkModelServiceProvider(this.StateManager),
                        serviceContext: context,
                        listenerBinding: new NetTcpBinding(SecurityMode.None),
                        address: new EndpointAddress(uri)
                    );
                    return listener;
                })
            };
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            await Task.Factory.StartNew(() => NetworkModelServiceProvider._networkModel = new FTN.Services.NetworkModelService.NetworkModel(this.StateManager));
        }
    }
}
