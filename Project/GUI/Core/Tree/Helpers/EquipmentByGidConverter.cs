using System.Collections.Generic;
using Core.Common.ServiceBus.Commands;

namespace GUI.Core.Tree.Helpers
{
    public static class EquipmentByGidConverter
    {
        public static Dictionary<long, EquipmentNodeItem> Convert(ModelUpdateCommand command)
        {
            var equipmentByGid = new Dictionary<long, EquipmentNodeItem>();

            // connectivity node
            foreach (var connectivityNode in command.ConnectivityNodes)
            {
                equipmentByGid[connectivityNode.GID] = new EquipmentNodeItem(connectivityNode.GetType(),
                    connectivityNode,
                    connectedTo: connectivityNode.Terminals,
                    hidden: true);
            }

            // terminal
            foreach (var terminal in command.Terminals)
            {
                equipmentByGid[terminal.GID] = new EquipmentNodeItem(terminal.GetType(),
                    terminal,
                    connectedTo: new[] { terminal.ConnectivityNode },
                    hidden: true);
            }

            // a. machine
            foreach (var item in command.AsynchronousMachines)
            {
                equipmentByGid[item.GID] = new EquipmentNodeItem(item.GetType(),
                    item,
                    connectedTo: item.Terminals);
            }

            // Breakers
            foreach (var breaker in command.Breakers)
            {
                equipmentByGid[breaker.GID] = new EquipmentNodeItem(breaker.GetType(),
                    breaker,
                    connectedTo: breaker.Terminals);
            }

            // Disconnectors
            foreach (var disconnector in command.Disconnectors)
            {
                equipmentByGid[disconnector.GID] = new EquipmentNodeItem(disconnector.GetType(),
                    disconnector,
                    connectedTo: disconnector.Terminals);
            }

            // Transformer
            foreach (var winding in command.TransformerWindings)
            {
                equipmentByGid[winding.GID] = new EquipmentNodeItem(winding.GetType(),
                    winding,
                    connectedTo: winding.Terminals,
                    hidden: true);
            }
            foreach (var powerTransformer in command.PowerTransformers)
            {
                equipmentByGid[powerTransformer.GID] = new EquipmentNodeItem(powerTransformer.GetType(),
                    powerTransformer,
                    connectedTo: powerTransformer.TransformerWindings,
                    hidden: true);
            }
            foreach (var ratioTapChanger in command.RatioTapChangers)
            {
                equipmentByGid[ratioTapChanger.GID] = new EquipmentNodeItem(ratioTapChanger.GetType(),
                    ratioTapChanger,
                    connectedTo: new[] { ratioTapChanger.TransformerWinding });
            }

            // Measurements 
            foreach (var analogMeasurement in command.Analogs)
            {
                equipmentByGid[analogMeasurement.GID] = new EquipmentNodeItem(analogMeasurement.GetType(),
                    analogMeasurement,
                    connectedTo: new[] { analogMeasurement.Terminals },
                    hidden: true);
            }
            foreach (var discreteMeasurement in command.Discretes)
            {
                equipmentByGid[discreteMeasurement.GID] = new EquipmentNodeItem(discreteMeasurement.GetType(),
                    discreteMeasurement,
                    connectedTo: new[] { discreteMeasurement.Terminals },
                    hidden: true);
            }

            return equipmentByGid;
        }
    }
}
