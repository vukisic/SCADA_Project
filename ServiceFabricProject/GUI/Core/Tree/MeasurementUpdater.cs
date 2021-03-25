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
        private object lockobj = new object();
        private Dictionary<string, string> flows;

        public MeasurementUpdater(FastLookupByMrid nodeLookupByMrid)
        {
            this.nodeLookupByMrid = nodeLookupByMrid;
        }

        public void UpdateValues(ScadaUpdateEvent updateEvent)
        {
            lock (lockobj)
            {
                flows = new Dictionary<string, string>();

                foreach (var point in updateEvent.Points)
                {
                    var nodeToUpdate = nodeLookupByMrid.Find(point.ObjectMrid);
                    if (nodeToUpdate == null)
                    {
                        continue;
                    }

                    if (point.Mrid.Contains("Flow"))
                    {
                        flows[point.Mrid] = point.Value.ToString();
                    }

                    nodeToUpdate.Item.UpdateMeasurements(Map(point));
                    if (point.RegisterType == RegisterType.ANALOG_OUTPUT && point.Mrid.Contains("Tap"))
                        nodeToUpdate.TurnedOn = nodeToUpdate.Active = point.Value > 0;
                    if (point.RegisterType == RegisterType.BINARY_OUTPUT)
                        nodeToUpdate.TurnedOn = nodeToUpdate.Active = point.Value == 1;

                }
                try
                {
                    if (updateEvent.Points.Count == 15)
                    {
                        var pump2 = nodeLookupByMrid.Find("AsyncM_2");
                        var flow2 = nodeLookupByMrid.Find("Flow_AM2");
                        if (pump2 == null)
                            return;
                        var list = new List<string>() { "Disc_01", "Breaker_01", "Disc_02", "Disc_12", "Breaker_12", "Disc_22", "RatioTC_2", "Breaker_22" };

                        var nodes = new List<EquipmentTreeNode>();
                        list.ForEach(x => nodes.Add(nodeLookupByMrid.Find(x)));

                        var result = nodes.Any(x => x.TurnedOn == false);
                        pump2.TurnedOn = !result;
                        pump2.Active = true;
                        pump2.SpecialValue = flows["Flow_AM2"];


                        if (result)
                        {
                            foreach (var item in nodes)
                            {
                                if (!item.TurnedOn || !item.Active)
                                {
                                    item.Active = false;
                                    foreach (var eq in item.Children)
                                    {
                                        eq.Active = false;
                                    }
                                }
                            }
                        }
                    }
                    else if (updateEvent.Points.Count == 24)
                    {
                        var pump1 = nodeLookupByMrid.Find("AsyncM_1");
                        var pump2 = nodeLookupByMrid.Find("AsyncM_2");
                        if (pump1 == null || pump2 == null)
                            return;
                        var list1 = new List<string>() { "Disc_01", "Breaker_01", "Disc_02", "Disc_11", "Breaker_11", "Disc_21", "RatioTC_1", "Breaker_21" };
                        var list2 = new List<string>() { "Disc_01", "Breaker_01", "Disc_02", "Disc_12", "Breaker_12", "Disc_22", "RatioTC_2", "Breaker_22" };

                        var nodes1 = new List<EquipmentTreeNode>();
                        list1.ForEach(x => nodes1.Add(nodeLookupByMrid.Find(x)));

                        var nodes2 = new List<EquipmentTreeNode>();
                        list2.ForEach(x => nodes2.Add(nodeLookupByMrid.Find(x)));

                        var result1 = nodes1.Any(x => x.TurnedOn == false);
                        pump1.TurnedOn = !result1;
                        pump1.Active = true;
                        pump1.SpecialValue = flows["Flow_AM1"];

                        var result2 = nodes2.Any(x => x.TurnedOn == false);
                        pump2.TurnedOn = !result2;
                        pump2.Active = true;
                        pump2.SpecialValue = flows["Flow_AM2"];

                        if (result1)
                        {
                            foreach (var item in nodes1)
                            {
                                if (!item.TurnedOn || !item.Active)
                                {
                                    item.Active = false;
                                    foreach (var eq in item.Children)
                                    {
                                        eq.Active = false;
                                    }
                                }
                            }
                        }

                        if (result2)
                        {
                            foreach (var item in nodes2)
                            {
                                if (!item.TurnedOn || !item.Active)
                                {
                                    item.Active = false;
                                    foreach (var eq in item.Children)
                                    {
                                        eq.Active = false;
                                    }
                                }
                            }
                        }
                    }
                    else if (updateEvent.Points.Count == 33)
                    {
                        var pump1 = nodeLookupByMrid.Find("AsyncM_1");
                        var pump2 = nodeLookupByMrid.Find("AsyncM_2");
                        var pump3 = nodeLookupByMrid.Find("AsyncM_3");
                        if (pump1 == null || pump2 == null || pump3 == null)
                            return;
                        var list1 = new List<string>() { "Disc_01", "Breaker_01", "Disc_02", "Disc_11", "Breaker_11", "Disc_21", "RatioTC_1", "Breaker_21" };
                        var list2 = new List<string>() { "Disc_01", "Breaker_01", "Disc_02", "Disc_12", "Breaker_12", "Disc_22", "RatioTC_2", "Breaker_22" };
                        var list3 = new List<string>() { "Disc_01", "Breaker_01", "Disc_02", "Disc_13", "Breaker_13", "Disc_23", "RatioTC_3", "Breaker_23" };

                        var nodes1 = new List<EquipmentTreeNode>();
                        list1.ForEach(x => nodes1.Add(nodeLookupByMrid.Find(x)));

                        var nodes2 = new List<EquipmentTreeNode>();
                        list2.ForEach(x => nodes2.Add(nodeLookupByMrid.Find(x)));

                        var nodes3 = new List<EquipmentTreeNode>();
                        list3.ForEach(x => nodes3.Add(nodeLookupByMrid.Find(x)));

                        var result1 = nodes1.Any(x => x.TurnedOn == false);
                        pump1.TurnedOn = !result1;
                        pump1.Active = true;
                        pump1.SpecialValue = flows["Flow_AM1"];

                        var result2 = nodes2.Any(x => x.TurnedOn == false);
                        pump2.TurnedOn = !result2;
                        pump2.Active = true;
                        pump2.SpecialValue = flows["Flow_AM2"];

                        var result3 = nodes3.Any(x => x.TurnedOn == false);
                        pump3.TurnedOn = !result3;
                        pump3.Active = true;
                        pump3.SpecialValue = flows["Flow_AM3"];

                        if (result1)
                        {
                            foreach (var item in nodes1)
                            {
                                if (!item.TurnedOn || !item.Active)
                                {
                                    item.Active = false;
                                    foreach (var eq in item.Children)
                                    {
                                        eq.Active = false;
                                    }
                                }
                            }
                        }

                        if (result2)
                        {
                            foreach (var item in nodes2)
                            {
                                if (!item.TurnedOn || !item.Active)
                                {
                                    item.Active = false;
                                    foreach (var eq in item.Children)
                                    {
                                        eq.Active = false;
                                    }
                                }
                            }
                        }

                        if (result3)
                        {
                            foreach (var item in nodes3)
                            {
                                if (!item.TurnedOn || !item.Active)
                                {
                                    item.Active = false;
                                    foreach (var eq in item.Children)
                                    {
                                        eq.Active = false;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception) { }
            }
        }

        private MeasurementModel Map(IScadaPointDto scadaPoint)
        {
            return new MeasurementModel(scadaPoint);
        }
    }
}
