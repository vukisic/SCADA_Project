using Azure.Messaging.ServiceBus;
using System;
using System.Threading.Tasks;

namespace Core.Common.PubSub
{
    public class Publisher : IDisposable
    {
        private string _connectionString;
        private string _topicName;
        private ServiceBusClient _client;
        private ServiceBusSender _sender;

        public Publisher(string topicName, string connectionString)
        {
            _connectionString = connectionString;
            _topicName = topicName;
            _client = new ServiceBusClient(connectionString);
            _sender = _client.CreateSender(topicName);
        }

        public Task SendMessage(PubSubMessage message)
        {
            return _sender.SendMessageAsync(new ServiceBusMessage(PubSubMessageSerializer.Serialize(message)));
        }

        public Task SendMessage(string message)
        {
            return _sender.SendMessageAsync(new ServiceBusMessage(message));
        }

        public void Dispose()
        {
            _sender.DisposeAsync().GetAwaiter().GetResult();
            _sender = null;
            _client.DisposeAsync().GetAwaiter().GetResult();
            _client = null;
        }
    }
}
