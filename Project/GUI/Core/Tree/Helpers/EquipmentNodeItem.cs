using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.ServiceBus.Dtos;

namespace GUI.Core.Tree.Helpers
{
    public class EquipmentNodeItem
    {
        public EquipmentNodeItem(Type itemType, IIdentifiedObject item, bool hidden = false, bool visited = false)
        {
            Type = itemType;
            Item = item;
            Hidden = hidden;
            Visited = visited;
        }
        public EquipmentNodeItem(Type itemType, IIdentifiedObject item, IEnumerable<long> connectedTo, bool hidden = false, bool visited = false)
            : this(itemType, item, hidden, visited)
        {
            ConnectedTo = connectedTo;
        }

        public Type Type { get; set; }
        public IIdentifiedObject Item { get; set; }
        public bool Hidden { get; set; }
        public bool Visited { get; set; }

        public IEnumerable<long> ConnectedTo { get; set; } = Enumerable.Empty<long>();
    }
}
