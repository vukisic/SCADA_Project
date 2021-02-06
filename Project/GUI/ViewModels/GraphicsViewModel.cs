using System.Collections.ObjectModel;
using System.Diagnostics;
using Caliburn.Micro;
using Core.Common.ServiceBus.Commands;
using Core.Common.ServiceBus.Events;
using GUI.Core.Tree;

namespace GUI.ViewModels
{
    public class GraphicsViewModel : Screen
    {
        private ObservableCollection<EquipmentTreeNode> nodes;

        public ObservableCollection<EquipmentTreeNode> Nodes
        {
            get { return nodes; }
            set
            {
                nodes = value;
                NotifyOfPropertyChange(() => Nodes);
            }
        }

        public void Update(object sender, ModelUpdateCommand e)
        {
            App.Current.Dispatcher.Invoke((System.Action)delegate
            {
                EquipmentTreeNode tree = EquipmentTreeFactory.CreateFrom(e);
                DisplayTree(tree);
            });
        }

        public void UpdateMeasurements(object sender, ScadaUpdateEvent e)
        {
            App.Current.Dispatcher.Invoke((System.Action)delegate
            {
                // Update code scada measurements
            });
        }

        private void DisplayTree(EquipmentTreeNode tree)
        {
            Debug.WriteLine("Displaying tree...");
            Nodes = new ObservableCollection<EquipmentTreeNode>(new[] { tree });
        }
    }
}
