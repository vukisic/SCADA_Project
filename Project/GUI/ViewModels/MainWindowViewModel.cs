using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Core.Common.ServiceBus.Commands;
using Core.Common.ServiceBus.Events;
using NServiceBus;

namespace GUI.ViewModels
{
    public class MainWindowViewModel : Conductor<object>, IHandleMessages<ModelUpdateCommand>, IHandleMessages<HistoryGraphicalEvent>, IHandleMessages<ScadaUpdateEvent>, IHandleMessages<DomUpdateEvent>, IHandleMessages<HistoryUpdateEvent>, IHandleMessages<CeUpdateEvent>, IHandleMessages<CeGraphicalEvent>
    {
        private static EventHandler<ScadaUpdateEvent> scadaUpdate = delegate { };
        private static EventHandler<DomUpdateEvent> domUpdate = delegate { };
        private static EventHandler<HistoryUpdateEvent> historyUpdate = delegate { };
        private static EventHandler<ModelUpdateCommand> modelUpdate = delegate { };
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
        public MainWindowViewModel()
        {
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

        public Task Handle(ScadaUpdateEvent message, IMessageHandlerContext context)
        {
            scadaUpdate.Invoke(this,message);
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

        public Task Handle(ModelUpdateCommand message, IMessageHandlerContext context)
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
