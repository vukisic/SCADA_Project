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
using MahApps.Metro.Controls;
using NServiceBus;
using SCADA.Common.DataModel;

namespace GUI.ViewModels
{
    public class ScadaDataViewModel : Conductor<object>
    {
        private static bool state;
        private IWindowManager manager;
        private IEndpointInstance endpoint;
        private int selected;
        private bool isOn;
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
            IsOn = state;
        }

        public int Selected
        {
            get { return selected; }
            set { selected = value; NotifyOfPropertyChange(() => Selected); }
        }

        public bool IsOn { get => isOn; set { isOn = value; NotifyOfPropertyChange(() => IsOn); } }

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

        public void OnToggle(ToggleSwitch sw) 
        {
            if (sw.IsOn)
                ONClick();
            else
                OFFClick();
        }

        public void ONClick()
        {
            state = true;
            ScadaExportProxy proxy = new ScadaExportProxy();
            var points = proxy.GetData();
            var commands = new List<ScadaCommandingEvent>();
            foreach (var item in points)
            {
                if (item.Key.Contains("Tap"))
                    continue;
                if(item.Key.Contains("Disc") || item.Key.Contains("Status"))
                {
                    if (!item.Key.Contains("Breaker_21") && !item.Key.Contains("Breaker_22") && !item.Key.Contains("Breaker_23"))
                    {
                        var command = new ScadaCommandingEvent()
                        {
                            Index = (uint)item.Value.Index,
                            RegisterType = item.Value.RegisterType,
                            Milliseconds = 0,
                            Value = 1
                        };
                        commands.Add(command);
                    }

                }
            }

            foreach (var item in commands)
            {
                endpoint.Publish(item).ConfigureAwait(false);
            }
            
        }

        public void OFFClick()
        {
            state = true;
            ScadaExportProxy proxy = new ScadaExportProxy();
            var points = proxy.GetData();
            var commands = new List<ScadaCommandingEvent>();
            foreach (var item in points)
            {
                if (item.Key.Contains("Tap"))
                    continue;
                if (item.Key.Contains("Disc") || item.Key.Contains("Status"))
                {
                    if (!item.Key.Contains("Breaker_21") && !item.Key.Contains("Breaker_22") && !item.Key.Contains("Breaker_23"))
                    {
                        var command = new ScadaCommandingEvent()
                        {
                            Index = (uint)item.Value.Index,
                            RegisterType = item.Value.RegisterType,
                            Milliseconds = 0,
                            Value = 0
                        };
                        commands.Add(command);
                    }
                }
            }

            foreach (var item in commands)
            {
                endpoint.Publish(item).ConfigureAwait(false);
            }
        }

    }
}
