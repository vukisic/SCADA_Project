using System;
using Core.Common.Json;
using Core.Common.PubSub;
using Core.Common.ServiceBus.Events;
using GUI.Core.Tree;
using GUI.Models.Schema;
using SCADA.Common;
using SF.Common.Proxies;

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
                SendMessageToScada(new ScadaCommandingEvent()
                {
                    Index = (uint)measurement.Index,
                    RegisterType = measurement.RegisterType,
                    Milliseconds = 0,
                    Value = (uint)(newTurnedOn ? 1 : 0)
                });
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
        }

        protected virtual void OnCanExecuteChanged(EventArgs e)
        {
            CanExecuteChanged?.Invoke(this, e);
        }

        private static void SendMessageToScada(ScadaCommandingEvent commandingEvent)
        {
            var proxy = new CommandingProxy();
            proxy.Commmand(new ScadaCommand(commandingEvent.RegisterType, commandingEvent.Index, commandingEvent.Value, 0)).GetAwaiter().GetResult();
        }
    }
}
