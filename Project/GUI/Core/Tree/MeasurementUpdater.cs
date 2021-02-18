using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.ServiceBus.Events;
using GUI.Core.Tree.Helpers;
using GUI.Models.Schema;
using SCADA.Common.DataModel;

namespace GUI.Core.Tree
{
    public class MeasurementUpdater
    {
        private readonly FastLookupByMrid nodeLookupByMrid;

        public MeasurementUpdater(FastLookupByMrid nodeLookupByMrid)
        {
            this.nodeLookupByMrid = nodeLookupByMrid;
        }

        public void UpdateValues(ScadaUpdateEvent updateEvent)
        {
            foreach (var point in updateEvent.Points)
            {
                var nodeToUpdate = nodeLookupByMrid.Find(point.ObjectMrid);
                if (nodeToUpdate == null)
                {
                    continue;
                }

                nodeToUpdate.Item.UpdateMeasurements(Map(point));
                if (point.RegisterType == RegisterType.ANALOG_OUTPUT && point.Mrid.Contains("Tap"))
                    nodeToUpdate.TurnedOn = point.Value > 0;

                // Logic for disable all child nodes 
                //if (point.RegisterType == RegisterType.BINARY_OUTPUT && !point.Mrid.Contains("Tap"))
                //{
                //    nodeToUpdate.TurnedOn = point.Value == 1 ? true : false;
                //}
                //UpdateChilds(nodeToUpdate, point);
            }

            if (updateEvent.Points.Count == 15)
            {
                var pump2 = nodeLookupByMrid.Find("AsyncM_2");
                var list = new List<string>() { "Disc_01", "Breaker_01", "Disc_02","Disc_12","Breaker_12","Disc_22","RatioTC_2","Breaker_22" };
                var nodes = new List<EquipmentTreeNode>();
                list.ForEach(x => nodes.Add(nodeLookupByMrid.Find(x)));
                var result = nodes.Any(x => x.TurnedOn == false);
                pump2.TurnedOn = !result;
            }
            else if (updateEvent.Points.Count == 24)
            {
                var pump1 = nodeLookupByMrid.Find("AsyncM_1");
                var pump2 = nodeLookupByMrid.Find("AsyncM_2");
                var list1 = new List<string>() { };
                var list2 = new List<string>() { };
                var nodes1 = new List<EquipmentTreeNode>();
                var nodes2 = new List<EquipmentTreeNode>();
            }
            else if (updateEvent.Points.Count == 33)
            {
                var pump1 = nodeLookupByMrid.Find("AsyncM_1");
                var pump2 = nodeLookupByMrid.Find("AsyncM_2");
                var pump3 = nodeLookupByMrid.Find("AsyncM_3");
                var list1 = new List<string>() { };
                var list2 = new List<string>() { };
                var list3 = new List<string>() { };
                var nodes1 = new List<EquipmentTreeNode>();
                var nodes2 = new List<EquipmentTreeNode>();
                var nodes3 = new List<EquipmentTreeNode>();

            }
        }

        private void UpdateChilds(EquipmentTreeNode nodeToUpdate, ScadaPointDto point)
        {
            if(point.RegisterType == RegisterType.BINARY_OUTPUT && !point.Mrid.Contains("Tap") && point.Value == 0)
            {
                DisableNode(nodeToUpdate);
            }
        }

        private void DisableNode (EquipmentTreeNode node)
        {
            if (node.Children != null && node.Children.Count > 0)
            {
                foreach (var item in node.Children)
                {
                    item.TurnedOn = false;
                }
            }
        }

        private MeasurementModel Map(IScadaPointDto scadaPoint)
        {
            return new MeasurementModel(scadaPoint);
        }
    }
}
