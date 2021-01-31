using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common.ServiceBus;
using NServiceBus;

namespace GUI.ServiceBus
{
    internal static class ServiceBusStartup
    {
        /// <summary>
        /// Creates and starts new instance for Scada endpoint
        /// </summary>
        /// <param name="endpointName"></param>
        public static Task<IEndpointInstance> StartInstance(string endpointName = EndpointNames.GUI)
        {
            var endpointConfiguration = GetConfiguration(endpointName);

            /* Start the endpoint */
            return Endpoint.Start(endpointConfiguration);
        }

        private static EndpointConfiguration GetConfiguration(string endpointName)
        {
            var endpointConfiguration = new EndpointConfiguration(endpointName);

            var transport = endpointConfiguration.UseTransport<LearningTransport>();
            var routing = transport.Routing();

            // Route example: 
            //routing.RouteToEndpoint(typeof(ScadaUpdateEvent), EndpointNames.GUI);
            // Note: you only need to define routes for commands (no need to do so for events!)

            return endpointConfiguration;
        }
    }
}
