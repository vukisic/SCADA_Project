using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Core.Common.ServiceBus.Commands;
using Core.Common.ServiceBus.Dtos;

namespace GUI.Core.Tree
{
    public static class EquipmentTreeFactory
    {
        public static EquipmentTreeNode CreateFrom(ModelUpdateCommand command)
        {
            // TODO: Implement this mapper
            Debug.WriteLine("Creating tree from command...");

            Dictionary<long, EquipmentNodeItem> equipmentNodeByGid = ConvertToEquipmentByGid(command);


            Debug.WriteLine("Inspecting connectivity nodes...");

            return new EquipmentTreeNode();
        }

        private static Dictionary<long, EquipmentNodeItem> ConvertToEquipmentByGid(ModelUpdateCommand command)
        {
            throw new NotImplementedException();
        }

        private class EquipmentNodeItem
        {
            public Type Type { get; set; }
            public IIdentifiedObject Item { get; set; }
            public bool Visited { get; set; }
            public bool Hidden { get; set; }

            public IEnumerable<long> ConnectedTo { get; set; } = Enumerable.Empty<long>();
        }
    }
}
