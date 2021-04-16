using Core.Common.WeatherApi;
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

namespace WeatherForecast
{
    internal sealed class WeatherForecast : StatelessService
    {
        public WeatherForecast(StatelessServiceContext context)
            : base(context)
        { }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new[] {new ServiceInstanceListener((context) =>
            {
                string host = host = context.NodeContext.IPAddressOrFQDN;

                EndpointResourceDescription endpointConfig = context.CodePackageActivationContext.GetEndpoint("ServiceEndPointW");

                int port = endpointConfig.Port;
                string scheme = endpointConfig.Protocol.ToString();
                string uri = string.Format(CultureInfo.InvariantCulture, "{0}://{1}:{2}/WeatherForecast", "net.tcp", host, port);

                var listener = new WcfCommunicationListener<IWeatherForecast>(
                    wcfServiceObject: new WeatherForecastProvider(this.Context),
                    serviceContext: context,
                    listenerBinding: new NetTcpBinding(SecurityMode.None),
                    address: new EndpointAddress(uri)

                );
                return listener;
            })};
        }
    }
}


