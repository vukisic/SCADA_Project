using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Contracts;
using Core.Common.PubSub;

namespace PubSubService
{
    public class PubSubServiceProvider : IPubSubAsync
    {
        public async Task Publish(PubSubMessage message)
        {
            Subscription subscription = new Subscription();
            using(var publisher = new Publisher(subscription.Topic, subscription.ConnectionString))
            {
                await publisher.SendMessage(message);
            }
        }
    }
}
