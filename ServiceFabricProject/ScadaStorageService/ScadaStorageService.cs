using Core.Common.Contracts;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace ScadaStorageService
{
    internal sealed class ScadaStorageService : StatefulService
    {
        public ScadaStorageService(StatefulServiceContext context)
            : base(context)
        { }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[] { new ServiceReplicaListener((context) =>
                {
                    var listener = new WcfCommunicationListener<IScadaStorageService>(
                        wcfServiceObject: new ScadaStorageServiceProvider(this.Context, this.StateManager),
                        serviceContext: context,
                        listenerBinding: new NetTcpBinding(SecurityMode.None),
                        endpointResourceName: "ServiceEndpointStorage"
                    );
                    return listener;
                })
            };
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            await ScadaStorageServiceProvider.Initialize();
        }
    }
}
