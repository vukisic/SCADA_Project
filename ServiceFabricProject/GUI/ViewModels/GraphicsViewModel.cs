using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;
using Core.Common.ServiceBus.Events;
using GUI.Core.Tree;
using GUI.Core.Tree.Helpers;
using GUI.Models.Schema;

namespace GUI.ViewModels
{
    public class GraphicsViewModel : Screen
    {
        private MeasurementUpdater measurementUpdater;
        private ObservableCollection<EquipmentTreeNode> nodes = new ObservableCollection<EquipmentTreeNode>();
        private ObservableCollection<TransformerModel> transformers = new ObservableCollection<TransformerModel>();
        private string level;

        public ObservableCollection<EquipmentTreeNode> Nodes
        {
            get { return nodes; }
            set
            {
                nodes = value;
                NotifyOfPropertyChange(() => Nodes);
            }
        }

        public ObservableCollection<TransformerModel> Transformers
        {
            get { return transformers; }
            set
            {
                transformers = value;
                NotifyOfPropertyChange(() => Transformers);
            }
        }

        public string Level
        {
            get { return level; }
            set
            {
                level = value;
                NotifyOfPropertyChange(() => Level);
            }
        }

        private bool IsModelRetrieved => Nodes?.Count > 0;

        public void Update(object sender, ModelUpdateEvent e)
        {
            EquipmentTreeNode root = EquipmentTreeFactory.CreateFrom(e);
            var fastNodeLookupByMrid = new FastLookupByMrid(root);

            App.Current.Dispatcher.Invoke((System.Action)delegate
            {
                measurementUpdater = new MeasurementUpdater(fastNodeLookupByMrid);
                DisplayTree(root);
                UpdateTransfomerList(root);
            });
        }

        public void UpdateMeasurements(object sender, ScadaUpdateEvent e)
        {
            e.Points = e.Points.Where(x => !string.IsNullOrEmpty(x.Mrid)).ToList();
            if (!IsModelRetrieved)
            {
                return;
            }

            App.Current.Dispatcher.Invoke((System.Action)delegate
            {
                measurementUpdater.UpdateValues(e);
                var lvl = e.Points.SingleOrDefault(x =>  x.Mrid.Contains("Level"));
                if (lvl != null)
                {
                    Level = $"Fluid Level: {lvl.Value}";
                }
            });
        }

        private void DisplayTree(EquipmentTreeNode root)
        {
            Nodes = new ObservableCollection<EquipmentTreeNode>(new[] { root });
        }

        private void UpdateTransfomerList(EquipmentTreeNode root)
        {
            var fastNodeLookupByItemType = new FastLookupByItemType(root);

            var transformers = fastNodeLookupByItemType
                .Find(typeof(TransformerModel))
                .Select((node) => node.Item as TransformerModel)
                .ToList();

            Transformers = new ObservableCollection<TransformerModel>(transformers);
        }
    }
}
