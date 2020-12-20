using System.Threading.Tasks;
using Core.Common.ServiceBus.Events;
using NServiceBus;
using NServiceBus.Logging;

namespace NDS.ServiceBus.Handlers
{
    public class DemoHandler : IHandleMessages<DemoEvent>
    {
        private static readonly ILog log = LogManager.GetLogger<DemoHandler>();

        public Task Handle(DemoEvent message, IMessageHandlerContext context)
        {
            log.Info($"Received DemoEvent, DemoProperty = {message.DemoProperty}");

            return Task.CompletedTask;
        }
    }
}
