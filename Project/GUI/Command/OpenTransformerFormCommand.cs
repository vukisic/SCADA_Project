using System;
using System.Diagnostics;
using System.Linq;
using Caliburn.Micro;
using Core.Common.ServiceBus.Events;
using GUI.Core.Tree;
using GUI.Models;
using GUI.Models.Schema;
using GUI.ServiceBus;
using GUI.ViewModels;
using NServiceBus;

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

            var endpoint = EndPointCreator.Instance().Get();
            var command = new ScadaCommandingEvent()
            {
                Index = (uint)formData.Index,
                RegisterType = formData.RegisterType.Value,
                Milliseconds = 0,
                Value = (uint)formData.Value
            };
            endpoint.Publish(command).ConfigureAwait(false);
        }

        protected virtual void OnCanExecuteChanged(EventArgs e)
        {
            CanExecuteChanged?.Invoke(this, e);
        }
    }
}
