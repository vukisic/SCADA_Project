using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core.Common.ServiceBus.Dtos;

namespace GUI.Core.Tree
{
    public class EquipmentTreeNode
    {
        public ObservableCollection<EquipmentTreeNode> Children { get; set; } = new ObservableCollection<EquipmentTreeNode>();
        public string Name { get; set; } = "";

        public Type Type { get; set; }
        public IIdentifiedObject Item { get; set; }
    }
}
