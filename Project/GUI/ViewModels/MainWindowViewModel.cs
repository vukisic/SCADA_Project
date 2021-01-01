using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Core.Common.ServiceBus.Events;
using NServiceBus;

namespace GUI.ViewModels
{
    public class MainWindowViewModel : Conductor<object>, IHandleMessages<ScadaUpdateEvent>, IHandleMessages<DomUpdateEvent>
    {
        private static EventHandler<ScadaUpdateEvent> scadaUpdate = delegate { };
        private static EventHandler<DomUpdateEvent> domUpdate = delegate { };
        private ScadaDataViewModel scada = new ScadaDataViewModel();
        private DOMViewModel dom = new DOMViewModel();
        private AlarmingViewModel alarms = new AlarmingViewModel();
        public MainWindowViewModel()
        {
            scadaUpdate += scada.Update;
            domUpdate += dom.Update;
            scadaUpdate += alarms.Update;
            LoadScadaDataView();
        }
        public void LoadGraphicsView()
        {
            ActivateItem(new GraphicsViewModel());
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
            ActivateItem(new AlarmingViewModel());
        }

        public void LoadHistoryView()
        {
            ActivateItem(new HistoryViewModel());
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
    }
}
