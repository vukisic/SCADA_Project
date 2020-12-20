using System.Threading.Tasks;
using Core.Common.ServiceBus;
using Core.Common.ServiceBus.Commands;
using NServiceBus;

namespace NDS.ServiceBus
{
    /// <summary>
    /// Helper class for initializing SCADA endpoint instance
    /// </summary>
    internal static class ServiceBusStartup
    {
        /// <summary>
        /// Creates and starts new instance for Scada endpoint
        /// </summary>
        /// <param name="endpointName"></param>
        public static Task<IEndpointInstance> StartInstance(string endpointName = EndpointNames.SCADA)
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
            // routing.RouteToEndpoint(typeof(DemoCommand), EndpointNames.GUI);
            // Note: you only need to define routes for commands (no need to do so for events!)

            return endpointConfiguration;
        }
    }
}
