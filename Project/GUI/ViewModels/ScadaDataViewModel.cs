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
using Core.Common.ServiceBus;
using Core.Common.ServiceBus.Events;
using FTN.Common;
using GUI.Core;
using GUI.Models;
using GUI.ServiceBus;
using NServiceBus;
using SCADA.Common.DataModel;

namespace GUI.ViewModels
{
    public class ScadaDataViewModel : Conductor<object>
    {
        private IWindowManager manager;
        private int selected;
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

        public int Selected
        {
            get { return selected; }
            set { selected = value; NotifyOfPropertyChange(() => Selected); }
        }

        public ScadaDataViewModel()
        {
            manager = IoC.Get<IWindowManager>();

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

        public void MouseDoubleClick()
        {
            if(Selected >= 0 && Selected <= Points.Count)
            {
                var item = Points[Selected];
                this.manager.ShowDialog(new ControlViewModel(item), null, null);
            }
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
