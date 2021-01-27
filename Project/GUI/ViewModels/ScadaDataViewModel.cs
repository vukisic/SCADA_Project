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

        public ScadaDataViewModel()
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

        public void UpdatePoints(List<ScadaPointDto> points)
        {
            App.Current.Dispatcher.Invoke((System.Action)delegate 
            {
                Data.Points.Clear();
                Points = new ObservableCollection<BasePointDto>();

                var result = (Core.Mapper.MapCollection<ScadaPointDto, BasePointDto>(points.ToList()));
                foreach (var item in result)
                {
                    Points.Add(item);
                }
            });
        }
    }
}
