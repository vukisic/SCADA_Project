using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Core.Common.ServiceBus.Dtos;
using GUI.Command;
using GUI.Core.Tree.Helpers;

namespace GUI.Core.Tree
{
    internal static class EquipmentTreeNodeFactory
    {
        internal static EquipmentTreeNode CreateNode(EquipmentNodeItem currentItem, List<EquipmentTreeNode> children)
        {
            var node = new EquipmentTreeNode
            {
                Children = new ObservableCollection<EquipmentTreeNode>(children),
                Name = currentItem.Item.Name,
                Type = currentItem.Type,
                Item = currentItem.Item
            };

            AttachHandlers(node);

            return node;
        }

        private static void AttachHandlers(EquipmentTreeNode node)
        {
            var typesWithElectricityToggleSupport = new[] { typeof(DisconnectorDto), typeof(BreakerDto) };

            Debug.WriteLine($"Node type = {node.Type}");

            if (typesWithElectricityToggleSupport.Any(type => type == node.Type))
            {
                node.OnClick = new ToggleElectricityCommand(node);
            }
        }
    }
}
