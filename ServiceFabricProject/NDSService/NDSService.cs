using Core.Common.Contracts;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Description;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace NDSService
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class NDSService : StatelessService
    {
        public NDSService(StatelessServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new[] {

            new ServiceInstanceListener((context) =>
            {
                string host = host = context.NodeContext.IPAddressOrFQDN;

                EndpointResourceDescription endpointConfig = context.CodePackageActivationContext.GetEndpoint("ServiceEndPointNDSModel");

                int port = endpointConfig.Port;
                string scheme = endpointConfig.Protocol.ToString();
                string uri = string.Format(CultureInfo.InvariantCulture, "{0}://{1}:{2}/NDSService", "net.tcp", host, port);

                var listener = new WcfCommunicationListener<IModelUpdateAsync>(
                    wcfServiceObject: new NDSModelProvider(this.Context),
                    serviceContext: context,
                    listenerBinding: new NetTcpBinding(SecurityMode.None),
                    address: new EndpointAddress(uri)

                );
                return listener;
            },"NDSModelProvider"),

            new ServiceInstanceListener((context) =>
            {

                string host = host = context.NodeContext.IPAddressOrFQDN;

                EndpointResourceDescription endpointConfig = context.CodePackageActivationContext.GetEndpoint("ServiceEndPointNDSTr");

                int port = endpointConfig.Port;
                string scheme = endpointConfig.Protocol.ToString();
                string uri = string.Format(CultureInfo.InvariantCulture, "{0}://{1}:{2}/NDSService", "net.tcp", host, port);

                var listener = new WcfCommunicationListener<ITransactionStepsAsync>(
                    wcfServiceObject: new NDSTransactionProvider(this.Context),
                    serviceContext: context,
                    listenerBinding: new NetTcpBinding(SecurityMode.None),
                    address: new EndpointAddress(uri)
                );
                return listener;
            },"NDSTransactionProvider")
        };
        }

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            long iterations = 0;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                ServiceEventSource.Current.ServiceMessage(this.Context, "Working-{0}", ++iterations);

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
