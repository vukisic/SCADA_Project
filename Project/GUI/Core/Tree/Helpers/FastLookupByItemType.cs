using System;
using System.Collections.Generic;
using System.Linq;

namespace GUI.Core.Tree.Helpers
{
    public class FastLookupByItemType
    {
        private readonly EquipmentTreeNode root;

        /// <summary>
        /// Generates fast lookup class which allows to find elements in O(1) time by type
        /// </summary>
        /// <param name="root">Root node of tree</param>
        public FastLookupByItemType(EquipmentTreeNode root)
        {
            this.root = root;

            UpdateNodeDictionary();
        }

        private Dictionary<Type, ICollection<EquipmentTreeNode>> nodesByItemType { get; } = new Dictionary<Type, ICollection<EquipmentTreeNode>>();

        /// <summary>
        /// Searches for all of the nodes where Item has specified type
        /// </summary>
        /// <param name="type"></param>
        /// <returns>Nodes where Item has specified type</returns>
        public IEnumerable<EquipmentTreeNode> Find(Type type)
        {
            if (nodesByItemType.TryGetValue(type, out var nodes))
            {
                return nodes;
            }

            return Enumerable.Empty<EquipmentTreeNode>();
        }

        private void UpdateNodeDictionary()
        {
            nodesByItemType.Clear();
            UpdateNodeDictionary(root);
        }

        private void UpdateNodeDictionary(EquipmentTreeNode currentNode)
        {
            var itemType = currentNode.Item.GetType();
            if (nodesByItemType.TryGetValue(itemType, out var nodes))
            {
                nodes.Add(currentNode);
            }
            else
            {
                nodesByItemType[itemType] = new List<EquipmentTreeNode> { currentNode };
            }

            foreach (var child in currentNode.Children)
            {
                UpdateNodeDictionary(child);
            }
        }
    }
}
