using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            return GenerateTree(equipmentNodeByGid, command.SourceGid);
        }

        private static EquipmentTreeNode GenerateTree(Dictionary<long, EquipmentNodeItem> equipmentNodeByGid, long rootGid)
        {
            throw new NotImplementedException();
        }
    }
}


