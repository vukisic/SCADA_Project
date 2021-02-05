using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CE.Common.Proxies;
using Core.Common.ServiceBus.Events;
using GUI.Models;
using GUI.ServiceBus;
using GUI.ViewModels;
using NServiceBus;

namespace GUI.Views
{
    /// <summary>
    /// Interaction logic for ScadaDataView.xaml
    /// </summary>
    public partial class ScadaDataView : UserControl
    {

        private IEndpointInstance endpoint;

        public ScadaDataView()
        {
            InitializeComponent();
            endpoint = EndPointCreator.Instance().Get();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            (DataContext as IDisposable).Dispose();
        }
        private void ON_Click(object sender, RoutedEventArgs e)
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

        private void OFF_Click(object sender, RoutedEventArgs e)
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
