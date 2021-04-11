using Azure.Messaging.ServiceBus;
using System;
using System.Threading.Tasks;

namespace Core.Common.PubSub
{
    public class Subscriber : IDisposable
    {
        private Subscription _subscription;
        private ServiceBusClient _client;
        private ServiceBusProcessor _processor;

        public Subscriber(Subscription subscription, Func<ProcessMessageEventArgs, Task> onMessage, Func<ProcessErrorEventArgs, Task> onError)
        {
            _subscription = subscription;
            _client = new ServiceBusClient(subscription.ConnectionString);
            _processor = _client.CreateProcessor(subscription.Topic, subscription.SubscriptionName, new ServiceBusProcessorOptions());
            _processor.ProcessMessageAsync += onMessage;
            _processor.ProcessErrorAsync += onError;
        }

        public Task Start()
        {
            return _processor.StartProcessingAsync();
        }

        public Task Stop()
        {
            return _processor.StopProcessingAsync();
        }

        public void Dispose()
        {
            _processor.DisposeAsync().GetAwaiter().GetResult();
            _processor = null;
            _client.DisposeAsync().GetAwaiter().GetResult();
            _client = null;
        }
    }
}
