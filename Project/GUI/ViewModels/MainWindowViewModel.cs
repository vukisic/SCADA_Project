using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace GUI.ViewModels
{
    public class MainWindowViewModel : Conductor<object>
    {
        public void LoadGraphicsView()
        {
            ActivateItem(new GraphicsViewModel());
        }

        public void LoadScadaDataView()
        {
            ActivateItem(new ScadaDataViewModel());
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
    }
}
