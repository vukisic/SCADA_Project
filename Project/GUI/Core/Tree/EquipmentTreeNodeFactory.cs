using System;
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
            AttachImage(node);

            return node;
        }

        private static void AttachImage(EquipmentTreeNode node)
        {
            var imageByType = new Dictionary<Type, string>
            {
                [typeof(DisconnectorDto)] = "/Images/Disconnector.png",
                [typeof(BreakerDto)] = "/Images/Breaker.png",
                [typeof(AsynchronousMachineDto)] = "/Images/AMachine.png",
                [typeof(TransformerWindingDto)] = "/Images/Transformer.png",
                [typeof(TransformerModel)] = "/Images/Transformer.png",
                [typeof(TerminalDto)] = "/Images/Terminal.png",
                [typeof(ConnectivityNodeDto)] = "/Images/ConnectivityNode.png"
            };

            if (imageByType.TryGetValue(node.Item.GetType(), out string imageSource))
            {
                node.ImageSource = imageSource;
            }
        }

        private static void AttachHandlers(EquipmentTreeNode node)
        {
            var typesWithElectricityToggleSupport = new[] { typeof(DisconnectorDto), typeof(BreakerDto) };

            Debug.WriteLine($"Node type = {node.Type}");

            if (typesWithElectricityToggleSupport.Any(type => type == node.Type))
            {
                node.OnClick = new ToggleElectricityCommand(node);
            }

            if (node.Item is TransformerModel)
            {
                node.OnClick = new OpenTransformerFormCommand(node);
            }
        }
    }
}
