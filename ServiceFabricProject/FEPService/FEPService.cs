using Core.Common.Contracts;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using NDS.FrontEnd;
using SCADA.Common;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace FEPService
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class FEPService : StatelessService
    {
        private IFEP _fep;
        public FEPService(StatelessServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new[] { new ServiceInstanceListener((context) =>
            {
                var listener = new WcfCommunicationListener<IFEPServiceAsync>(
                    wcfServiceObject: new FEPProvider(this.Context, AddCommand),
                    serviceContext: context,
                    listenerBinding: new NetTcpBinding(SecurityMode.None),
                    endpointResourceName: "ServiceEndpointFep" // iz ServiceManifest.xml
                );
                return listener;
                })
            };
        }

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            await Task.Run(() => {
                _fep = new FEP();
                _fep.Start();
            });
        }

        public Task AddCommand(ScadaCommand command)
        {
            _fep.ExecuteCommand(command);
            return Task.CompletedTask;
        }
    }
}
