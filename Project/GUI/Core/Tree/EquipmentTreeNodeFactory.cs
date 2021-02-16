using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Core.Common.ServiceBus.Dtos;
using GUI.Command;
using GUI.Core.Tree.Helpers;
using GUI.Models.Schema;
using GUI.Models.Schema.Helpers;

namespace GUI.Core.Tree
{
    internal static class EquipmentTreeNodeFactory
    {
        internal static EquipmentTreeNode CreateNode(EquipmentNodeItem currentItem, List<EquipmentTreeNode> children)
        {
            var dtoType = currentItem.Type;

            var node = new EquipmentTreeNode
            {
                Children = new ObservableCollection<EquipmentTreeNode>(children),
                Name = currentItem.Item.Name,
                Item = IdentifiedObjectToSchemaModelMapper.Map(currentItem.Item, dtoType)
            };

            AttachHandlers(node, dtoType);
            AttachImage(node, dtoType);

            return node;
        }

        private static void AttachImage(EquipmentTreeNode node, Type dtoType)
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

            if (imageByType.TryGetValue(dtoType, out string imageSource))
            {
                node.ImageSource = imageSource;
            }
        }

        private static void AttachHandlers(EquipmentTreeNode node, Type dtoType)
        {
            var typesWithElectricityToggleSupport = new[] { typeof(DisconnectorDto), typeof(BreakerDto) };

            if (typesWithElectricityToggleSupport.Any(type => type == dtoType))
            {
                node.OnClick = new ToggleElectricityCommand(node);
            }

            if (dtoType == typeof(TransformerModel))
            {
                node.OnClick = new OpenTransformerFormCommand(node);
            }
        }
    }
}
