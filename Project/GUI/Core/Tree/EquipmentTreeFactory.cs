using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Core.Common.ServiceBus.Commands;
using GUI.Core.Tree.Helpers;

namespace GUI.Core.Tree
{
    public static class EquipmentTreeFactory
    {
        public static EquipmentTreeNode CreateFrom(ModelUpdateCommand command)
        {
            Debug.WriteLine("Creating tree from command...");

            Dictionary<long, EquipmentNodeItem> equipmentNodeByGid = EquipmentByGidConverter.Convert(command);

            var nodes = GetTreeNodes(equipmentNodeByGid, command.SourceGid);
            return nodes.FirstOrDefault();
        }

        private static IEnumerable<EquipmentTreeNode> GetTreeNodes(Dictionary<long, EquipmentNodeItem> equipmentNodeByGid, long nodeId)
        {
            if (!equipmentNodeByGid.TryGetValue(nodeId, out var currentItem) || currentItem.Visited)
            {
                return Enumerable.Empty<EquipmentTreeNode>();
            }

            currentItem.Visited = true;

            var children = new List<EquipmentTreeNode>();
            foreach (long neighborId in currentItem.ConnectedTo)
            {
                if (!equipmentNodeByGid.TryGetValue(neighborId, out var neighborItem) || neighborItem.Visited)
                {
                    continue;
                }
                var neighbours = GetTreeNodes(equipmentNodeByGid, neighborId);
                if (neighbours.Any())
                {
                    children.AddRange(neighbours);
                }
            }

            if (currentItem.Hidden)
            {
                return children;
            }

            return new[] {
                new EquipmentTreeNode{
                    Children = new ObservableCollection<EquipmentTreeNode>(children),
                    Name = currentItem.Item.Name,
                    Type = currentItem.Type,
                    Item = currentItem.Item
                }
            };
        }
    }
}


