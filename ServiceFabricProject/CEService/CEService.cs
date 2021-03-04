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

namespace CEService
{
	/// <summary>
	/// An instance of this class is created for each service instance by the Service Fabric runtime.
	/// </summary>
	internal sealed class CEService : StatelessService
	{
		public CEService(StatelessServiceContext context)
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

                    EndpointResourceDescription endpointConfig = context.CodePackageActivationContext.GetEndpoint("ServiceEndPoint");
                    
                    int port = endpointConfig.Port;
                    string scheme = endpointConfig.Protocol.ToString();
                    string uri = string.Format(CultureInfo.InvariantCulture, "{0}://{1}:{2}/CEService", "net.tcp", host, port);

                    var listener = new WcfCommunicationListener<IModelUpdateAsync>(
                        wcfServiceObject: new CEModelProvider(this.Context),
                        serviceContext: context,
                        listenerBinding: new NetTcpBinding(SecurityMode.None),
                        address: new EndpointAddress(uri)

                    );
                    return listener;
                },"CEModelProvider"),

                new ServiceInstanceListener((context) =>
                {

                    string host = host = context.NodeContext.IPAddressOrFQDN;

                    EndpointResourceDescription endpointConfig = context.CodePackageActivationContext.GetEndpoint("ServiceEndPointTR");

                    int port = endpointConfig.Port;
                    string scheme = endpointConfig.Protocol.ToString();
                    string uri = string.Format(CultureInfo.InvariantCulture, "{0}://{1}:{2}/CEService", "net.tcp", host, port);

                    var listener = new WcfCommunicationListener<ITransactionStepsAsync>(
                        wcfServiceObject: new CETransactionProvider(this.Context),
                        serviceContext: context,
                        listenerBinding: new NetTcpBinding(SecurityMode.None),
                        address: new EndpointAddress(uri)
                    );
                    return listener;
                },"CETransactionProvider")
            };
        }

	}
}
