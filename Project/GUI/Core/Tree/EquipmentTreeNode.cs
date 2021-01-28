using System;
using System.Collections.Generic;
using Core.Common.ServiceBus.Dtos;

namespace GUI.Core.Tree
{
    public class EquipmentTreeNode
    {
        public IEnumerable<EquipmentTreeNode> Children { get; set; } = new List<EquipmentTreeNode>();
        public string Name { get; set; } = "";

        public Type Type { get; set; }
        public IIdentifiedObject Item { get; set; }
    }
}
