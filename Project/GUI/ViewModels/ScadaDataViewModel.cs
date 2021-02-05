using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using CE.Common.Proxies;
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
        private IEndpointInstance endpoint;
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
        public ScadaDataViewModel()
        {
            manager = IoC.Get<IWindowManager>();

            Points = new ObservableCollection<BasePointDto>();

            endpoint = EndPointCreator.Instance().Get();
        }

        public int Selected
        {
            get { return selected; }
            set { selected = value; NotifyOfPropertyChange(() => Selected); }
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
                this.manager.ShowWindow(new ControlViewModel(item, endpoint), null, null);
            }
        }

        public void UpdatePoints(List<ScadaPointDto> points)
        {
            App.Current.Dispatcher.Invoke((System.Action)delegate 
            {
                Points = null;
                var tempPoints = new ObservableCollection<BasePointDto>();

                var result = (Core.Mapper.MapCollection<ScadaPointDto, BasePointDto>(points.ToList()));
                foreach (var item in result)
                {
                    tempPoints.Add(item);
                }
                Points = tempPoints;
            });
        }
        public void ONClick()
        {
            ScadaExportProxy proxy = new ScadaExportProxy();
            var points = proxy.GetData();

            var disc01 = points[$"Discrete_Disc01"];
            var breaker01 = points[$"Breaker_01Status"];
            var disc02 = points[$"Discrete_Disc02"];

            var dis11 = points[$"Discrete_Disc11"];
            var breaker11 = points[$"Breaker_11Status"];
            var dis21 = points[$"Discrete_Disc21"];

            var dis12 = points[$"Discrete_Disc12"];
            var breaker12 = points[$"Breaker_12Status"];
            var dis22 = points[$"Discrete_Disc22"];

            var dis13 = points[$"Discrete_Disc13"];
            var breaker13 = points[$"Breaker_13Status"];
            var dis23 = points[$"Discrete_Disc23"];

            var command1 = new ScadaCommandingEvent()
            {
                Index = (uint)disc01.Index,
                RegisterType = disc01.RegisterType,
                Milliseconds = 0,
                Value = 1
            };

            var command2 = new ScadaCommandingEvent()
            {
                Index = (uint)disc02.Index,
                RegisterType = disc02.RegisterType,
                Milliseconds = 0,
                Value = 1
            };

            var command3 = new ScadaCommandingEvent()
            {
                Index = (uint)breaker01.Index,
                RegisterType = breaker01.RegisterType,
                Milliseconds = 0,
                Value = 1
            };

            var command4 = new ScadaCommandingEvent()
            {
                Index = (uint)dis11.Index,
                RegisterType = dis11.RegisterType,
                Milliseconds = 0,
                Value = 1
            };

            var command5 = new ScadaCommandingEvent()
            {
                Index = (uint)dis12.Index,
                RegisterType = dis12.RegisterType,
                Milliseconds = 0,
                Value = 1
            };

            var command6 = new ScadaCommandingEvent()
            {
                Index = (uint)dis13.Index,
                RegisterType = dis13.RegisterType,
                Milliseconds = 0,
                Value = 1
            };
            var command7 = new ScadaCommandingEvent()
            {
                Index = (uint)dis21.Index,
                RegisterType = dis21.RegisterType,
                Milliseconds = 0,
                Value = 1
            };

            var command8 = new ScadaCommandingEvent()
            {
                Index = (uint)dis22.Index,
                RegisterType = dis22.RegisterType,
                Milliseconds = 0,
                Value = 1
            };

            var command9 = new ScadaCommandingEvent()
            {
                Index = (uint)dis23.Index,
                RegisterType = dis23.RegisterType,
                Milliseconds = 0,
                Value = 1
            };

            var command10 = new ScadaCommandingEvent()
            {
                Index = (uint)breaker11.Index,
                RegisterType = breaker11.RegisterType,
                Milliseconds = 0,
                Value = 1
            };

            var command11 = new ScadaCommandingEvent()
            {
                Index = (uint)breaker12.Index,
                RegisterType = breaker12.RegisterType,
                Milliseconds = 0,
                Value = 1
            };

            var command12 = new ScadaCommandingEvent()
            {
                Index = (uint)breaker13.Index,
                RegisterType = breaker13.RegisterType,
                Milliseconds = 0,
                Value = 1
            };

            endpoint.Publish(command1).ConfigureAwait(false);
            endpoint.Publish(command2).ConfigureAwait(false);
            endpoint.Publish(command3).ConfigureAwait(false);
            endpoint.Publish(command4).ConfigureAwait(false);
            endpoint.Publish(command5).ConfigureAwait(false);
            endpoint.Publish(command6).ConfigureAwait(false);
            endpoint.Publish(command7).ConfigureAwait(false);
            endpoint.Publish(command8).ConfigureAwait(false);
            endpoint.Publish(command9).ConfigureAwait(false);
            endpoint.Publish(command10).ConfigureAwait(false);
            endpoint.Publish(command11).ConfigureAwait(false);
            endpoint.Publish(command12).ConfigureAwait(false);
        }

        public void OFF_Click()
        {
            ScadaExportProxy proxy = new ScadaExportProxy();
            var points = proxy.GetData();

            var disc01 = points[$"Discrete_Disc01"];
            var breaker01 = points[$"Breaker_01Status"];
            var disc02 = points[$"Discrete_Disc02"];

            var dis11 = points[$"Discrete_Disc11"];
            var breaker11 = points[$"Breaker_11Status"];
            var dis21 = points[$"Discrete_Disc21"];

            var dis12 = points[$"Discrete_Disc12"];
            var breaker12 = points[$"Breaker_12Status"];
            var dis22 = points[$"Discrete_Disc22"];

            var dis13 = points[$"Discrete_Disc13"];
            var breaker13 = points[$"Breaker_13Status"];
            var dis23 = points[$"Discrete_Disc23"];

            var command1 = new ScadaCommandingEvent()
            {
                Index = (uint)disc01.Index,
                RegisterType = disc01.RegisterType,
                Milliseconds = 0,
                Value = 0
            };

            var command2 = new ScadaCommandingEvent()
            {
                Index = (uint)disc02.Index,
                RegisterType = disc02.RegisterType,
                Milliseconds = 0,
                Value = 0
            };

            var command3 = new ScadaCommandingEvent()
            {
                Index = (uint)breaker01.Index,
                RegisterType = breaker01.RegisterType,
                Milliseconds = 0,
                Value = 0
            };

            var command4 = new ScadaCommandingEvent()
            {
                Index = (uint)dis11.Index,
                RegisterType = dis11.RegisterType,
                Milliseconds = 0,
                Value = 0
            };

            var command5 = new ScadaCommandingEvent()
            {
                Index = (uint)dis12.Index,
                RegisterType = dis12.RegisterType,
                Milliseconds = 0,
                Value = 0
            };

            var command6 = new ScadaCommandingEvent()
            {
                Index = (uint)dis13.Index,
                RegisterType = dis13.RegisterType,
                Milliseconds = 0,
                Value = 0
            };
            var command7 = new ScadaCommandingEvent()
            {
                Index = (uint)dis21.Index,
                RegisterType = dis21.RegisterType,
                Milliseconds = 0,
                Value = 0
            };

            var command8 = new ScadaCommandingEvent()
            {
                Index = (uint)dis22.Index,
                RegisterType = dis22.RegisterType,
                Milliseconds = 0,
                Value = 0
            };

            var command9 = new ScadaCommandingEvent()
            {
                Index = (uint)dis23.Index,
                RegisterType = dis23.RegisterType,
                Milliseconds = 0,
                Value = 0
            };

            var command10 = new ScadaCommandingEvent()
            {
                Index = (uint)breaker11.Index,
                RegisterType = breaker11.RegisterType,
                Milliseconds = 0,
                Value = 0
            };

            var command11 = new ScadaCommandingEvent()
            {
                Index = (uint)breaker12.Index,
                RegisterType = breaker12.RegisterType,
                Milliseconds = 0,
                Value = 0
            };

            var command12 = new ScadaCommandingEvent()
            {
                Index = (uint)breaker13.Index,
                RegisterType = breaker13.RegisterType,
                Milliseconds = 0,
                Value = 0
            };

            endpoint.Publish(command1).ConfigureAwait(false);
            endpoint.Publish(command2).ConfigureAwait(false);
            endpoint.Publish(command3).ConfigureAwait(false);
            endpoint.Publish(command4).ConfigureAwait(false);
            endpoint.Publish(command5).ConfigureAwait(false);
            endpoint.Publish(command6).ConfigureAwait(false);
            endpoint.Publish(command7).ConfigureAwait(false);
            endpoint.Publish(command8).ConfigureAwait(false);
            endpoint.Publish(command9).ConfigureAwait(false);
            endpoint.Publish(command10).ConfigureAwait(false);
            endpoint.Publish(command11).ConfigureAwait(false);
            endpoint.Publish(command12).ConfigureAwait(false);
        }

    }
}
