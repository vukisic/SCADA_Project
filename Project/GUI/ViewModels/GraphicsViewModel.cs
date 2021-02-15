using System.Collections.ObjectModel;
using Caliburn.Micro;
using Core.Common.ServiceBus.Commands;
using Core.Common.ServiceBus.Events;
using GUI.Core.Tree;
using GUI.Core.Tree.Helpers;

namespace GUI.ViewModels
{
    public class GraphicsViewModel : Screen
    {
        private MeasurementUpdater measurementUpdater;
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

        private bool IsModelRetrieved => Nodes?.Count > 0;

        public void Update(object sender, ModelUpdateCommand e)
        {
            App.Current.Dispatcher.Invoke((System.Action)delegate
            {
                EquipmentTreeNode root = EquipmentTreeFactory.CreateFrom(e);

                var fastNodeLookupByMrid = new FastLookupByMrid(root);
                measurementUpdater = new MeasurementUpdater(fastNodeLookupByMrid);

                Nodes = new ObservableCollection<EquipmentTreeNode>(new[] { root });
            });
        }

        public void UpdateMeasurements(object sender, ScadaUpdateEvent e)
        {
            App.Current.Dispatcher.Invoke((System.Action)delegate
            {
                if (!IsModelRetrieved)
                {
                    return;
                }

                measurementUpdater.UpdateValues(e);
            });
        }
    }
}
