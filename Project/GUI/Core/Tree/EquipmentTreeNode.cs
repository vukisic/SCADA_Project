using System.Collections.Generic;

namespace GUI.Core.Tree
{
    public class EquipmentTreeNode
    {
        public ICollection<EquipmentTreeNode> Children { get; set; } = new List<EquipmentTreeNode>();
        public string Name { get; set; } = "";
    }
}
