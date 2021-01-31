using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Common.ServiceBus.Events;
using NServiceBus;
using SCADA.Common.DataModel;

namespace NDS.ProcessingModule
{
    public class CommandingService : IDisposable, IHandleMessages<ScadaCommandingEvent>
    {
        public static IProcessingManager processingManager;
        private Thread commandingWorker;
        private Dictionary<Tuple<RegisterType, uint>, CeCommand> calculationEngineCommands;
        public CommandingService()
        {
            calculationEngineCommands = new Dictionary<Tuple<RegisterType, uint>, CeCommand>();
            InitializeCommandingServiceThread();
            StartCommandingServiceThread();
        }

        private void InitializeCommandingServiceThread()
        {
            commandingWorker = new Thread(Commanding_DoWork);
            commandingWorker.Name = "Commanding thread";
        }
        
        private void StartCommandingServiceThread()
        {
            commandingWorker.Start();
        }

        private void Commanding_DoWork()
        {
            while (true)
            {
                //commandingTrigger.WaitOne();
                try
                {
                    Thread.Sleep(1000);
                    foreach (KeyValuePair<Tuple<RegisterType, uint>, CeCommand> item in calculationEngineCommands)
                    {
                        if (item.Value.MillisecondsPassedSinceLastPoll == item.Value.Milliseconds)
                        {
                            processingManager.ExecuteWriteCommand(item.Value.RegisterType, item.Value.Index, item.Value.Value);
                            calculationEngineCommands.Remove(item.Key);
                        }
                        item.Value.MillisecondsPassedSinceLastPoll += 1000;
                    }
                }
                catch(Exception)
                {

                }
                
            }
        }

        public void Dispose()
        {
            commandingWorker.Abort();
        }

        public Task Handle(ScadaCommandingEvent message, IMessageHandlerContext context)
        {
            calculationEngineCommands.Add(new Tuple<RegisterType, uint>(message.RegisterType, message.Index), new CeCommand
            {
                RegisterType = message.RegisterType,
                Index = message.Index,
                Value = message.Value,
                Milliseconds = message.Milliseconds,
                MillisecondsPassedSinceLastPoll = 0
            });
            return Task.CompletedTask;
        }
    }
}
