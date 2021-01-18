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
    public class MainWindowViewModel : Conductor<object>, IHandleMessages<ModelUpdateCommand>, IHandleMessages<ScadaUpdateEvent>, IHandleMessages<DomUpdateEvent>, IHandleMessages<HistoryUpdateEvent>, IHandleMessages<CeUpdateEvent>
    {
        private static EventHandler<ScadaUpdateEvent> scadaUpdate = delegate { };
        private static EventHandler<DomUpdateEvent> domUpdate = delegate { };
        private static EventHandler<HistoryUpdateEvent> historyUpdate = delegate { };
        private static EventHandler<ModelUpdateCommand> modelUpdate = delegate { };
        private static EventHandler<CeUpdateEvent> ceUpdate = delegate { };
        private ScadaDataViewModel scada = new ScadaDataViewModel();
        private DOMViewModel dom = new DOMViewModel();
        private AlarmingViewModel alarms = new AlarmingViewModel();
        private HistoryViewModel history = new HistoryViewModel();
        private GraphicsViewModel graphics = new GraphicsViewModel();
        private CEDataViewModel ce = new CEDataViewModel();
        public MainWindowViewModel()
        {
            scadaUpdate += scada.Update;
            scadaUpdate += graphics.UpdateMeasurements;
            domUpdate += dom.Update;
            scadaUpdate += alarms.Update;
            historyUpdate += history.Update;
            modelUpdate += graphics.Update;
            ceUpdate += ce.Update;
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
            ActivateItem(new CEDataViewModel());
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
    }
}
