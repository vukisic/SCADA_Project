using System;
using Core.Common.ServiceBus.Events;
using GUI.Core.Tree;
using GUI.Models.Schema;
using GUI.ServiceBus;
using NServiceBus;

namespace GUI.Command
{
    public class ToggleElectricityCommand : System.Windows.Input.ICommand
    {
        private readonly EquipmentTreeNode _node;

        public ToggleElectricityCommand(EquipmentTreeNode node)
        {
            _node = node;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (!_node.IsClickable)
            {
                return;
            }

            bool newTurnedOn = !_node.TurnedOn;

            if (_node.Item is SwitchModel model)
            {
                var measurement = model.Measurement;
                var endpoint = EndPointCreator.Instance().Get();
                var command = new ScadaCommandingEvent()
                {
                    Index = (uint)measurement.Index,
                    RegisterType = measurement.RegisterType,
                    Milliseconds = 0,
                    Value = (uint)(newTurnedOn ? 1 : 0)
                };
                endpoint.Publish(command).ConfigureAwait(false);
            }

            SetElectricity(_node, newTurnedOn, isRoot: true);
        }

        public void SetElectricity(EquipmentTreeNode node, bool value, bool isRoot = false)
        {
            if (!isRoot)
            {
                node.IsClickable = value;
            }

            node.TurnedOn = value;

            foreach (var child in node.Children)
            {
                SetElectricity(child, value);
            }
        }

        protected virtual void OnCanExecuteChanged(EventArgs e)
        {
            CanExecuteChanged?.Invoke(this, e);
        }
    }
}
