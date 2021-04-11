using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Contracts;
using Core.Common.PubSub;

namespace PubSubCommunicationService
{
    public class PubSubProvider : IPubSubAsync
    {
        private StatelessServiceContext _context;

        public PubSubProvider(StatelessServiceContext context)
        {
            _context = context;
        }

        public async Task Publish(PubSubMessage message)
        {
            Subscription subscription = new Subscription();
            using(var pub = new Publisher(subscription.Topic, subscription.ConnectionString))
            {
                await pub.SendMessage(message);
            }
        }
    }
}
