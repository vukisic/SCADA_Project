using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Core.Common.ServiceBus.Events;
using GUI.Core;
using GUI.Models;
using SCADA.Common.DataModel;

namespace GUI.ViewModels
{
    public class AlarmingViewModel : Screen
    {
        private ObservableCollection<BasePointDto> _points;
        public ObservableCollection<BasePointDto> Points
        {
            get { return _points; }
            set
            {
                _points = value;
                NotifyOfPropertyChange(() => Points);
            }
        }

        public AlarmingViewModel()
        {
            Points = new ObservableCollection<BasePointDto>();
            foreach (var item in Data.Points)
            {
                if(item.Alarm != SCADA.Common.DataModel.AlarmType.NO_ALARM)
                    Points.Add(item);
            }
        }

        public void Update(object sender, ScadaUpdateEvent e)
        {
            UpdatePoints(e.Points);
        }

        private void UpdatePoints(List<ScadaPointDto> points)
        {
            App.Current.Dispatcher.Invoke((System.Action)delegate
            {
                Points = new ObservableCollection<BasePointDto>();

                var result = (Core.Mapper.MapCollection<ScadaPointDto,BasePointDto>(points.Where(x=>x.Alarm != AlarmType.NO_ALARM).ToList()));
                foreach (var item in result)
                {
                    Points.Add(item);
                }
            });
        }

    }
}
