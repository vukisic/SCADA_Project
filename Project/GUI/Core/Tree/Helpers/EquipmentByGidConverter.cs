using System.Collections.Generic;
using Core.Common.ServiceBus.Commands;
using Core.Common.ServiceBus.Dtos;
using GUI.Models.Schema;

namespace GUI.Core.Tree.Helpers
{
    public static class EquipmentByGidConverter
    {
        public static Dictionary<long, EquipmentNodeItem> Convert(ModelUpdateCommand command)
        {
            var equipmentByGid = new Dictionary<long, EquipmentNodeItem>();

            RegisterConnectivityNodes(command, equipmentByGid);
            RegisterTerminals(command, equipmentByGid);
            RegisterAsynchronousMachines(command, equipmentByGid);
            RegisterBreakers(command, equipmentByGid);
            RegisterDisconnectors(command, equipmentByGid);
            RegisterTransformers(command, equipmentByGid);
            RegisterMeasurements(command, equipmentByGid);

            return equipmentByGid;
        }

        private static void RegisterConnectivityNodes(ModelUpdateCommand command, Dictionary<long, EquipmentNodeItem> equipmentByGid)
        {
            foreach (var connectivityNode in command.ConnectivityNodes)
            {
                equipmentByGid[connectivityNode.GID] = new EquipmentNodeItem(connectivityNode.GetType(),
                    connectivityNode,
                    connectedTo: connectivityNode.Terminals,
                    hidden: true);
            }
        }

        private static void RegisterTerminals(ModelUpdateCommand command, Dictionary<long, EquipmentNodeItem> equipmentByGid)
        {
            foreach (var terminal in command.Terminals)
            {
                equipmentByGid[terminal.GID] = new EquipmentNodeItem(terminal.GetType(),
                    terminal,
                    connectedTo: new[] { terminal.ConnectivityNode, terminal.ConductingEquipment },
                    hidden: true);
            }
        }

        private static void RegisterAsynchronousMachines(ModelUpdateCommand command, Dictionary<long, EquipmentNodeItem> equipmentByGid)
        {
            foreach (var item in command.AsynchronousMachines)
            {
                equipmentByGid[item.GID] = new EquipmentNodeItem(item.GetType(),
                    item,
                    connectedTo: item.Terminals);
            }
        }

        private static void RegisterBreakers(ModelUpdateCommand command, Dictionary<long, EquipmentNodeItem> equipmentByGid)
        {
            foreach (var breaker in command.Breakers)
            {
                equipmentByGid[breaker.GID] = new EquipmentNodeItem(breaker.GetType(),
                    breaker,
                    connectedTo: breaker.Terminals);
            }
        }

        private static void RegisterDisconnectors(ModelUpdateCommand command, Dictionary<long, EquipmentNodeItem> equipmentByGid)
        {
            foreach (var disconnector in command.Disconnectors)
            {
                equipmentByGid[disconnector.GID] = new EquipmentNodeItem(disconnector.GetType(),
                    disconnector,
                    connectedTo: disconnector.Terminals);
            }
        }

        private static void RegisterMeasurements(ModelUpdateCommand command, Dictionary<long, EquipmentNodeItem> equipmentByGid)
        {
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
        }

        private static void RegisterTransformers(ModelUpdateCommand command, Dictionary<long, EquipmentNodeItem> equipmentByGid)
        {
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
                    connectedTo: new[] { ratioTapChanger.TransformerWinding },
                    hidden: true);
            }

            foreach (var winding in command.TransformerWindings)
            {
                var tapChanger = equipmentByGid[winding.RatioTapChanger].Item as RatioTapChangerDto;
                var powerTransformer = equipmentByGid[winding.PowerTransformer].Item as PowerTransformerDto;
                var transformer = new TransformerModel(winding, tapChanger, powerTransformer);

                equipmentByGid[winding.GID] = new EquipmentNodeItem(winding.GetType(),
                    transformer,
                    connectedTo: winding.Terminals);
            }
        }
    }
}
