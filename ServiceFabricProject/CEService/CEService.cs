using CE;
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
    internal sealed class CEService : StatelessService
    {
        public CEWorker _ceWorker;

        public CEService(StatelessServiceContext context)
            : base(context)
        { }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new[] { new ServiceInstanceListener((context) =>
                {
                    var listener = new WcfCommunicationListener<ICEServiceAsync>(
                        wcfServiceObject: new CEServiceProvider(this.UpdatePoints),
                        serviceContext: context,
                        listenerBinding: new NetTcpBinding(SecurityMode.None),
                        endpointResourceName: "ServiceEndpointCE"
                    );
                    return listener;

                })
            };
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                _ceWorker = new CEWorker();
                _ceWorker.Start();
            });
        }

        public Task UpdatePoints(int points)
        {
            _ceWorker.OnPointUpdate(points);
            return Task.CompletedTask;
        }
    }
}
