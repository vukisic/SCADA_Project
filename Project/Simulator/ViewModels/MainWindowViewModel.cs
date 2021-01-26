using Caliburn.Micro;
using Core.Common.Contracts;
using Simulator.Core;
using Simulator.Core.Model;
using Simulator.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.ViewModels
{
    public class MainWindowViewModel : Conductor<object>
    {
        private IWindowManager _windowManager;
        private ISimulator _simulator;
        private IMessageService _messageService;
        public event EventHandler<ClickEventArgs> _applyEvent;
        private ObservableCollection<Point> _points;
        private ServiceHost serviceHost;
        private Point _selected;
        private static bool started;
        public ObservableCollection<Point> Points
        {
            get { return _points; }
            set { _points = value; NotifyOfPropertyChange(() => Points);}
        }

       
        public Point Selected 
        { 
            get {return _selected; }
            set { _selected = value; NotifyOfPropertyChange(() => Selected); }
        }

        

        public MainWindowViewModel(IWindowManager windowManager, IMessageService messageService)
        {
            _messageService = messageService;
            _windowManager = windowManager;
            _applyEvent += MainWindowViewModel_applyEvent;
            Points = new ObservableCollection<Point>();
            if (!started)
            {
                ISimulatorConfiguration config = new SimulatorConfiguration();
                serviceHost = new ServiceHost(typeof(Simulator.Core.ConfigurationService));
                serviceHost.AddServiceEndpoint(typeof(IConfigurationChange), new NetTcpBinding(), new Uri("net.tcp://localhost:30000/IConfigurationChange"));
                serviceHost.Open();
                _simulator = new Core.Simulator(config);
                _simulator.updateEvent += Simulator_updateEvent;
                _simulator.Start();
                started = true;
            }
           
        }

        private void MainWindowViewModel_applyEvent(object sender, ClickEventArgs e)
        {
            var val = e.Point;
            if(e.Point.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_INPUT)
            {
                SingleInt32Union value = new SingleInt32Union();
                value.f = (e.Point as AnalogPoint).Value;
                _simulator.UpdatePoint((ushort)e.Point.Index, dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_INPUT, dnp3_protocol.tgttypes.eDataSizes.FLOAT32_SIZE, dnp3_protocol.tgtcommon.eDataTypes.FLOAT32_DATA, value);
            }
            else if(e.Point.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_OUTPUTS)
            {
                SingleInt32Union value = new SingleInt32Union();
                value.f = (e.Point as AnalogPoint).Value;
                _simulator.UpdatePoint((ushort)e.Point.Index, dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_OUTPUTS, dnp3_protocol.tgttypes.eDataSizes.FLOAT32_SIZE, dnp3_protocol.tgtcommon.eDataTypes.FLOAT32_DATA, value);
            }
            else if(e.Point.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.BINARY_INPUT)
            {
                SingleInt32Union value = new SingleInt32Union();
                value.i = (e.Point as BinaryPoint).Value;
                _simulator.UpdatePoint((ushort)e.Point.Index, dnp3_protocol.dnp3types.eDNP3GroupID.BINARY_INPUT, dnp3_protocol.tgttypes.eDataSizes.SINGLE_POINT_SIZE, dnp3_protocol.tgtcommon.eDataTypes.SINGLE_POINT_DATA, value);
            }
            else if(e.Point.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.BINARY_OUTPUT)
            {

                SingleInt32Union value = new SingleInt32Union();
                value.i = (e.Point as BinaryPoint).Value;
                _simulator.UpdatePoint((ushort)e.Point.Index, dnp3_protocol.dnp3types.eDNP3GroupID.BINARY_OUTPUT, dnp3_protocol.tgttypes.eDataSizes.SINGLE_POINT_SIZE, dnp3_protocol.tgtcommon.eDataTypes.SINGLE_POINT_DATA, value);
            }
        }

        private void Simulator_updateEvent(object sender, UpdateEventArgs e)
        {
            App.Current.Dispatcher.Invoke((System.Action)delegate
            {
                Points = new ObservableCollection<Point>();
                foreach (var item in e.Points)
                {
                    Points.Add(item);
                }
            });
        }

        public void OnClose(CancelEventArgs args)
        {
            _simulator.Stop();
            serviceHost.Close();
        }

        public void OnClick()
        {
            _windowManager.ShowDialog(new ControlWindowViewModel(Selected, _applyEvent,_messageService), null, null);
        }
    }
}
