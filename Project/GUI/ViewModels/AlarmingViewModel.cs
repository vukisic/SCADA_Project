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

        private void UpdatePoints(List<BasePoint> points)
        {
            App.Current.Dispatcher.Invoke((System.Action)delegate
            {
                Points = new ObservableCollection<BasePointDto>();
                foreach (var item in points)
                {
                    if (item.RegisterType == RegisterType.ANALOG_INPUT || item.RegisterType == RegisterType.ANALOG_OUTPUT)
                    {
                        if (item.Alarm != SCADA.Common.DataModel.AlarmType.NO_ALARM)
                            Points.Add(Mapper.Map<AnalogPointDto>(item));
                    }
                    else
                    {
                        if (item.Alarm != SCADA.Common.DataModel.AlarmType.NO_ALARM)
                            Points.Add(Mapper.Map<DiscretePointDto>(item));
                    }

                }
            });
        }

    }
}
