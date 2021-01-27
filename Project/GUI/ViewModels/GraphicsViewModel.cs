using System;
using System.Diagnostics;
using Caliburn.Micro;
using Core.Common.ServiceBus.Commands;
using Core.Common.ServiceBus.Events;
using GUI.Core.Tree;

namespace GUI.ViewModels
{
    public class GraphicsViewModel : Screen
    {
        internal void Update(object sender, ModelUpdateCommand e)
        {
            App.Current.Dispatcher.Invoke((System.Action)delegate
            {
                EquipmentTreeNode tree = EquipmentTreeFactory.CreateFrom(e);
                DisplayTree(tree);
            });
        }

        private void DisplayTree(EquipmentTreeNode tree)
        {
            // TODO: Display tree
            Debug.WriteLine("TODO: Displaying tree...");
        }

        internal void UpdateMeasurements(object sender, ScadaUpdateEvent e)
        {
            App.Current.Dispatcher.Invoke((System.Action)delegate
            {
                // Update code scada measurements
            });
        }
    }
}
