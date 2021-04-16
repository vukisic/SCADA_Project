using System;
using System.Diagnostics;
using System.Linq;
using Caliburn.Micro;
using Core.Common.Json;
using Core.Common.PubSub;
using Core.Common.ServiceBus.Events;
using GUI.Core.Tree;
using GUI.Models;
using GUI.Models.Schema;
using GUI.ViewModels;
using SCADA.Common;
using SF.Common.Proxies;

namespace GUI.Command
{
    public class OpenTransformerFormCommand : System.Windows.Input.ICommand
    {
        private readonly EquipmentTreeNode node;

        public OpenTransformerFormCommand(EquipmentTreeNode node)
        {
            this.node = node;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return node?.Item is TransformerModel;
        }

        public void Execute(object parameter)
        {
            if (!HasMeasurements())
            {
                return;
            }

            var windowManager = IoC.Get<IWindowManager>();

            var transformerModel = node.Item as TransformerModel;
            var formViewModel = new TransformerFormViewModel(transformerModel, onSubmit: UpdateNode);

            windowManager.ShowDialog(formViewModel);
        }

        protected virtual void OnCanExecuteChanged(EventArgs e)
        {
            CanExecuteChanged?.Invoke(this, e);
        }

        private bool HasMeasurements()
        {
            if (node?.Item is TransformerModel model)
            {
                return model.RatioTapChanger.Measurements.Any(measurement => measurement.MeasurementType == FTN.Common.MeasurementType.Discrete);
            }

            return false;
        }

        private void UpdateNode(TransformerFormData formData)
        {
            if (formData.Index is null || formData.RegisterType is null)
            {
                Debug.Fail("Index or RegisterType in form is NULL");
                return;
            };

            SendMessageToScada(new ScadaCommandingEvent()
            {
                Index = (uint)formData.Index,
                RegisterType = formData.RegisterType.Value,
                Milliseconds = 0,
                Value = (uint)formData.Value
            });
        }

        private static void SendMessageToScada(ScadaCommandingEvent commandingEvent)
        {
            var proxy = new CommandingProxy();
            proxy.Commmand(new ScadaCommand(commandingEvent.RegisterType, commandingEvent.Index, commandingEvent.Value, 0)).GetAwaiter().GetResult();
        }
    }
}
