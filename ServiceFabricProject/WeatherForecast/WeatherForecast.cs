using Core.Common.WeatherApi;
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

namespace WeatherForecast
{
    internal sealed class WeatherForecast : StatelessService
    {
        public WeatherForecast(StatelessServiceContext context)
            : base(context)
        { }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new[] { new ServiceInstanceListener((context) =>
                {
                    var listener = new WcfCommunicationListener<IWeatherForecast>(
                        wcfServiceObject: new WeatherForecastProvider(this.Context),
                        serviceContext: context,
                        listenerBinding: new NetTcpBinding(SecurityMode.None),
                        endpointResourceName: "ServiceEndpointW"
                    );
                    return listener;

                })
            };
        }
    }
}
