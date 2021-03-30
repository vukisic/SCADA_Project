using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace AlarmingService
{
    public class AlarmingService : StatelessService
    {
        public AlarmingService(StatelessServiceContext context)
            : base(context)
        { }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new[] { new ServiceInstanceListener((context) =>
                {
                    var listener = new WcfCommunicationListener<IAlarmingService>(
                        wcfServiceObject: new AlarmingServiceProvider(this.Context),
                        serviceContext: context,
                        listenerBinding: new NetTcpBinding(SecurityMode.None),
                        endpointResourceName: "ServiceEndpointAlarming"
                    );
                    return listener;

                })
            };
        }
    }
}
