using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common.ServiceBus;
using Core.Common.ServiceBus.Commands;
using NServiceBus;

namespace FTN.Services.NetworkModelService
{
    internal static class NMSServiceBus
    {
        /// <summary>
        /// Creates and starts new instance for NMS endpoint
        /// </summary>
        /// <param name="endpointName"></param>
        public static Task<IEndpointInstance> StartInstance(string endpointName = EndpointNames.NMS)
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
            routing.RouteToEndpoint(typeof(ModelUpdateCommand), EndpointNames.GUI);
            endpointConfiguration.SendOnly();

            // Route example: 
            // routing.RouteToEndpoint(typeof(DemoCommand), EndpointNames.GUI);
            // Note: you only need to define routes for commands (no need to do so for events!)

            return endpointConfiguration;
        }
    }
}
