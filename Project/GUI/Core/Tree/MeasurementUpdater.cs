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
            }
        }

        private MeasurementModel Map(IScadaPointDto scadaPoint)
        {
            return new MeasurementModel(scadaPoint);
        }
    }
}
