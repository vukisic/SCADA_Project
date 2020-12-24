using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Caliburn.Micro;
using Core.Common.ServiceBus.Events;
using FTN.Common;
using GUI.Core;
using GUI.Models;
using NServiceBus;
using SCADA.Common.DataModel;

namespace GUI.ViewModels
{
    public class ScadaDataViewModel : Conductor<object>
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

        public ScadaDataViewModel(EventHandler<ScadaUpdateEvent> eventHandler)
        {
            Points = new ObservableCollection<BasePointDto>();
            foreach (var item in Data.Points)
            {
                Points.Add(item);
            }
        }

        public void Update(object sender, ScadaUpdateEvent e)
        {
            UpdatePoints(e.Points);
        }

        public void UpdatePoints(List<BasePoint> points)
        {
            App.Current.Dispatcher.Invoke((System.Action)delegate 
            {
                Data.Points.Clear();
                Points = new ObservableCollection<BasePointDto>();
                foreach (var item in points)
                {
                    if (item.RegisterType == RegisterType.ANALOG_INPUT || item.RegisterType == RegisterType.ANALOG_OUTPUT)
                    {
                        Points.Add(Mapper.Map<AnalogPointDto>(item));
                        Data.Points.Add(Mapper.Map<AnalogPointDto>(item));
                    }
                    else
                    {
                        Points.Add(Mapper.Map<DiscretePointDto>(item));
                        Data.Points.Add(Mapper.Map<DiscretePointDto>(item));
                    }

                }
            });
        }
    }
}
