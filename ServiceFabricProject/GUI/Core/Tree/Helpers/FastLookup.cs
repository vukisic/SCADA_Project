using System;
using System.Collections.Generic;

namespace GUI.Core.Tree.Helpers
{
    public class FastLookup<TKey>
    {
        private readonly Func<EquipmentTreeNode, IEnumerable<TKey>> keysSelector;
        private readonly EquipmentTreeNode root;

        /// <summary>
        /// Generates fast lookup class which allows to find elements in O(1) time by given key
        /// </summary>
        /// <param name="root">Root node of tree</param>
        /// <param name="keysSelector">function that returns keys by which to search fast</param>
        public FastLookup(EquipmentTreeNode root, Func<EquipmentTreeNode, IEnumerable<TKey>> keysSelector)
        {
            this.root = root;
            this.keysSelector = keysSelector;

            UpdateNodeDictionary();
        }

        private Dictionary<TKey, EquipmentTreeNode> nodeDict { get; } = new Dictionary<TKey, EquipmentTreeNode>();

        /// <summary>
        /// Tries to get value by given key, if not found returns null
        /// </summary>
        /// <param name="key">key to search by</param>
        /// <returns>Node if found, null if not</returns>
        public EquipmentTreeNode Find(TKey key)
        {
            if (key != null && nodeDict.TryGetValue(key, out var node))
            {
                return node;
            }

            return null;
        }

        private void UpdateNodeDictionary()
        {
            nodeDict.Clear();
            UpdateNodeDictionary(root);
        }

        private void UpdateNodeDictionary(EquipmentTreeNode currentNode)
        {
            foreach (var key in keysSelector(currentNode))
            {
                if (key != null)
                {
                    nodeDict[key] = currentNode;
                }
            }

            foreach (var child in currentNode.Children)
            {
                UpdateNodeDictionary(child);
            }
        }
    }
}
