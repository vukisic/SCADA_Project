using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Core.Common.ServiceBus.Events;
using GUI.Command;
using GUI.Models;
using GUI.ServiceBus;
using NServiceBus;
using SCADA.Common.DataModel;

namespace GUI.ViewModels
{
    public class ControlViewModel : Screen
    {
        public MyICommand WriteCommand { get; set; }
        private int commandedValue;
        public int CommandedValue { get => commandedValue; set { commandedValue = value; NotifyOfPropertyChange(() => CommandedValue); } }

        private BasePointDto model;
        public BasePointDto Model { get => model; set { model = value; NotifyOfPropertyChange(() => Model); } }
        public ControlViewModel(BasePointDto dto)
        {
            Model = dto;
            WriteCommand = new MyICommand(OnWrite, CanWrite);
        }

        private void OnWrite(object obj)
        {
            try
            {
                IEndpointInstance instance = ServiceBusStartup.StartInstance()
                   .ConfigureAwait(false)
                   .GetAwaiter()
                   .GetResult();

                ScadaCommandingEvent ev = new ScadaCommandingEvent()
                {
                    Index = (uint)Model.Index,
                    Milliseconds = 0,
                    RegisterType = Model.RegisterType,
                    Value = (uint)CommandedValue
                };

                instance.Publish(ev).ConfigureAwait(false);
                TryClose();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private bool CanWrite(object obj)
        {
            if (Model.RegisterType == RegisterType.ANALOG_OUTPUT)
                return !(CommandedValue < Model.MinValue || CommandedValue > Model.MaxValue);
            else if (Model.RegisterType == RegisterType.BINARY_OUTPUT)
                return !(CommandedValue < 0 || CommandedValue > 1);
            else
                return false;
        }
    }
}
