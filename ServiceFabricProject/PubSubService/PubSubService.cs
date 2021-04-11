using System.Collections.Generic;
using System.Fabric;
using System.ServiceModel;
using Core.Common.Contracts;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace PubSubService
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class PubSubService : StatelessService
    {
        public PubSubService(StatelessServiceContext context)
            : base(context)
        { }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new[] { new ServiceInstanceListener((context) =>
            {
                var listener = new WcfCommunicationListener<IPubSubAsync>(
                    wcfServiceObject: new PubSubServiceProvider(this.Context),
                    serviceContext: context,
                    listenerBinding: new NetTcpBinding(SecurityMode.None),
                    endpointResourceName: "ServiceEndpointPubSub" // from ServiceManifest.xml
                );
                return listener;
            })
        };
        }
    }
}
