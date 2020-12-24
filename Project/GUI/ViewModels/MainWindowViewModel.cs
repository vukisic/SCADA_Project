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
    public class MainWindowViewModel : Conductor<object>, IHandleMessages<ScadaUpdateEvent>
    {
        private static EventHandler<ScadaUpdateEvent> scadaUpdate = delegate { };
        private ScadaDataViewModel scada = new ScadaDataViewModel(scadaUpdate);
        public MainWindowViewModel()
        {
            scadaUpdate += scada.Update;
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
            ActivateItem(new DOMViewModel());
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
    }
}
