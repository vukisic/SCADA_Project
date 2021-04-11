using System.Fabric;
using System.Threading.Tasks;
using Core.Common.Contracts;
using Core.Common.PubSub;

namespace PubSubService
{
    public class PubSubServiceProvider : IPubSubAsync
    {
        private StatelessServiceContext _context;

        public PubSubServiceProvider(StatelessServiceContext context)
        {
            _context = context;
        }

        public async Task Publish(PubSubMessage message)
        {
            Subscription sub = new Subscription();
            using (Publisher publisher = new Publisher(sub.Topic, sub.ConnectionString))
            {
                await publisher.SendMessage(message);
            }
        }
    }
}
