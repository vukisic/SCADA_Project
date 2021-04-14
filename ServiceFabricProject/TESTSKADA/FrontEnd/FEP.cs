using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Common.ServiceBus.Events;
using NDS.ProcessingModule;
using NServiceBus;
using SCADA.Common;
using SCADA.Common.Connection;
using SCADA.Common.DataModel;
using SCADA.Common.Messaging;
using SCADA.Common.Messaging.Parameters;
using SCADA.Common.Proxies;
using SCADA.Common.ScadaServices;

namespace NDS.FrontEnd
{
    public class FEP : IFEP, IHandleMessages<ScadaCommandingEvent>
    {
        private static bool started = false;
        private Acquisitor acquisitor;
        private static CommandingService commandingService;
        private IProcessingManager processingManager;
        private IFunctionExecutor functionExecutor;
        public FEP()
        {
                      
        }

        public Task Handle(ScadaCommandingEvent message, IMessageHandlerContext context)
        {
            commandingService.commanding?.Invoke(this, message);
            return Task.CompletedTask;
        }

        public void Start()
        {
            if (!started)
            {
                functionExecutor = new FunctionExecutor();
                processingManager = new ProcessingManager(functionExecutor);
                acquisitor = new Acquisitor(processingManager);
                commandingService = new CommandingService(processingManager);
                started = true;
            }
        }

        public void ExecuteCommand(ScadaCommand command)
        {
            processingManager.ExecuteWriteCommand(command.RegisterType, command.Index, command.Value);
        }
    }
}
