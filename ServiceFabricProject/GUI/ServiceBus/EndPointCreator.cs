using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NServiceBus;

namespace GUI.ServiceBus
{
    public class EndPointCreator
    {
        private static EndPointCreator instance;
        private IEndpointInstance endpoint;
        private EndPointCreator()
        {

        }


        public static EndPointCreator Instance()
        {
            if (instance == null)
            {
                instance = new EndPointCreator();
            }
            return instance;
        }

        public IEndpointInstance Get()
        {
            if (endpoint == null)
            {
                endpoint = ServiceBusStartup.StartInstance().ConfigureAwait(false).GetAwaiter().GetResult();
            }
            return endpoint;
        }
    }
}
