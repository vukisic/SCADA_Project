using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Caliburn.Micro;
using Core.Common.Json;
using Core.Common.PubSub;
using Core.Common.ServiceBus.Events;
using NServiceBus;

namespace GUI.ViewModels
{
    public class MainWindowViewModel : Conductor<object>
    {
        private static EventHandler<ScadaUpdateEvent> scadaUpdate = delegate { };
        private static EventHandler<DomUpdateEvent> domUpdate = delegate { };
        private static EventHandler<HistoryUpdateEvent> historyUpdate = delegate { };
        private static EventHandler<ModelUpdateEvent> modelUpdate = delegate { };
        private static EventHandler<CeUpdateEvent> ceUpdate = delegate { };
        private static EventHandler<HistoryGraphicalEvent> graphUpdate = delegate { };
        private static EventHandler<CeGraphicalEvent> ceUpdatePumpsValues = delegate { };

        private ScadaDataViewModel scada = new ScadaDataViewModel();
        private DOMViewModel dom = new DOMViewModel();
        private AlarmingViewModel alarms = new AlarmingViewModel();
        private HistoryViewModel history = new HistoryViewModel();
        private GraphicsViewModel graphics = new GraphicsViewModel();
        private CEDataViewModel ce = new CEDataViewModel();
        private HistoryGraphViewModel graph = new HistoryGraphViewModel();
        private Subscriber sub;

        public MainWindowViewModel()
        {
            Subscription subscription = new Subscription();
            subscription.SubscriptionName = "guisub";
            sub = new Subscriber(subscription, onMessage, onError);
            sub.Start().GetAwaiter().GetResult();
            scadaUpdate += scada.Update;
            scadaUpdate += graphics.UpdateMeasurements;
            domUpdate += dom.Update;
            scadaUpdate += alarms.Update;
            historyUpdate += history.Update;
            modelUpdate += graphics.Update;
            ceUpdate += ce.Update;
            graphUpdate += graph.Update;
            ceUpdatePumpsValues += ce.UpdatePumpsValues;
            LoadScadaDataView();
        }

        public void LoadGraphicsView()
        {
            ActivateItem(graphics);
        }

        public void LoadScadaDataView()
        {
            ActivateItem(scada);
        }

        public void LoadDomView()
        {
            ActivateItem(dom);
        }

        public void LoadAlarmingView()
        {
            ActivateItem(alarms);
        }

        public void LoadHistoryView()
        {
            ActivateItem(history);
        }

        public void CEDataView()
        {
            ActivateItem(ce);
        }

        public void HistoryGraphView()
        {
            ActivateItem(graph);
        }

        public Task onError(ProcessErrorEventArgs arg)
        {
            Console.WriteLine(arg.Exception.ToString());
            return Task.CompletedTask;
        }

        public Task onMessage(ProcessMessageEventArgs arg)
        {
            string body = arg.Message.Body.ToString();
            var message = JsonTool.Deserialize<PubSubMessage>(body);
            switch (message.ContentType)
            {
                case ContentType.NMS_UPDATE: { var obj = JsonTool.Deserialize<ModelUpdateEvent>(message.Content); Handle(obj, null); break; }
                case ContentType.SCADA_UPDATE: { var obj = JsonTool.Deserialize<ScadaUpdateEvent>(message.Content); Handle(obj, null); break; }
                case ContentType.SCADA_HISTORY: { var obj = JsonTool.Deserialize<HistoryUpdateEvent>(message.Content); Handle(obj, null); break; }
                case ContentType.SCADA_DOM: { var obj = JsonTool.Deserialize<DomUpdateEvent>(message.Content); Handle(obj, null); break; }
                case ContentType.SCADA_HISTORY_GRAPH: { var obj = JsonTool.Deserialize<HistoryGraphicalEvent>(message.Content); Handle(obj, null); break; }
                case ContentType.CE_UPDATE: { var obj = JsonTool.Deserialize<CeUpdateEvent>(message.Content); Handle(obj, null); break; }
                case ContentType.CE_HISTORY_GRAPH: { var obj = JsonTool.Deserialize<CeGraphicalEvent>(message.Content); Handle(obj, null); break; }
                default: break;
            }
            return arg.CompleteMessageAsync(arg.Message);
        }

        public Task Handle(ScadaUpdateEvent message, IMessageHandlerContext context)
        {
            scadaUpdate.Invoke(this, message);
            return Task.CompletedTask;
        }

        public Task Handle(DomUpdateEvent message, IMessageHandlerContext context)
        {
            domUpdate.Invoke(this, message);
            return Task.CompletedTask;
        }

        public Task Handle(HistoryUpdateEvent message, IMessageHandlerContext context)
        {
            historyUpdate.Invoke(this, message);
            return Task.CompletedTask;
        }

        public Task Handle(ModelUpdateEvent message, IMessageHandlerContext context)
        {
            modelUpdate.Invoke(this, message);
            return Task.CompletedTask;
        }

        public Task Handle(CeUpdateEvent message, IMessageHandlerContext context)
        {
            ceUpdate.Invoke(this, message);
            return Task.CompletedTask;
        }

        public Task Handle(HistoryGraphicalEvent message, IMessageHandlerContext context)
        {
            graphUpdate.Invoke(this, message);
            return Task.CompletedTask;
        }

        public Task Handle(CeGraphicalEvent message, IMessageHandlerContext context)
        {
            ceUpdatePumpsValues.Invoke(this, message);
            return Task.CompletedTask;
        }
    }
}
