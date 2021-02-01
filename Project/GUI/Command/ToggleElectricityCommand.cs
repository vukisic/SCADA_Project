using System;
using System.Windows.Input;
using GUI.Core.Tree;

namespace GUI.Command
{
    public class ToggleElectricityCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private readonly EquipmentTreeNode _node;

        public ToggleElectricityCommand(EquipmentTreeNode node)
        {
            _node = node;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            SetElectricity(_node, !_node.TurnedOn);
        }

        public void SetElectricity(EquipmentTreeNode node, bool value)
        {
            node.TurnedOn = value;

            foreach (var child in node.Children)
            {
                SetElectricity(child, value);
            }
        }
    }
}
