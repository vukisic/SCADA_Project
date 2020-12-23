using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Core.Common.ServiceBus.Events;
using FTN.Common;
using GUI.Core;
using GUI.Models;
using NServiceBus;
using SCADA.Common.DataModel;

namespace GUI.ViewModels
{
    public class ScadaDataViewModel : Conductor<object>, IHandleMessages<ScadaUpdateEvent>
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

        public Task Handle(ScadaUpdateEvent message, IMessageHandlerContext context)
        {
            UpdatePoints(message.Points);
            return Task.CompletedTask;
        }

        private void UpdatePoints(List<BasePoint> points)
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
        }
    }
}
