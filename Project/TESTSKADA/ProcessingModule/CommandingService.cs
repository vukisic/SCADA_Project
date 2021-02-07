using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Common.ServiceBus.Events;
using NServiceBus;
using SCADA.Common.DataModel;
using SCADA.Common.Proxies;

namespace NDS.ProcessingModule
{
    public class CommandingService : IDisposable
    {
        public IProcessingManager processingManager;
        private Thread commandingWorker;
        private ConcurrentBag<CeCommand> calculationEngineCommands;
        public EventHandler<ScadaCommandingEvent> commanding = delegate { };
        public CommandingService(IProcessingManager processingManager)
        {
            commanding += UpdateEvent;
            this.processingManager = processingManager;
            calculationEngineCommands = new ConcurrentBag<CeCommand>();
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
                try
                {
                    Thread.Sleep(1000);
                    foreach (var item in calculationEngineCommands)
                    {
                        if (item.MillisecondsPassedSinceLastPoll >= item.Milliseconds)
                        {
                            processingManager.ExecuteWriteCommand(item.RegisterType, item.Index, item.Value);
                            item.Remove = true;
                        }
                        item.MillisecondsPassedSinceLastPoll += 1000;
                    }
                    calculationEngineCommands = new ConcurrentBag<CeCommand>(calculationEngineCommands.Where(x => x.Remove == false).ToList());
                }
                catch(Exception e)
                {
                    ScadaProxyFactory.Instance().LoggingProxy().Log(new SCADA.Common.Logging.LogEventModel()
                    {
                        EventType = SCADA.Common.Logging.LogEventType.ERROR,
                        Message = $"{e.Message} - {e.StackTrace}"
                    }) ;
                }
                
            }
        }

        public void UpdateEvent(object sender, ScadaCommandingEvent message)
        {
            calculationEngineCommands.Add(new CeCommand
            {
                RegisterType = message.RegisterType,
                Index = message.Index,
                Value = message.Value,
                Milliseconds = message.Milliseconds,
                MillisecondsPassedSinceLastPoll = 0,
                Remove = false
            }) ;
        }

        public void Dispose()
        {
            commandingWorker.Abort();
        }
    }
}
