using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common.ServiceBus.Commands;
using NServiceBus;

namespace GUI
{
    public class ModelUpdateHandler : IHandleMessages<ModelUpdateCommand>
    {
        public Task Handle(ModelUpdateCommand message, IMessageHandlerContext context)
        {
            var model = message.Model;

            return null;
        }
    }
}
