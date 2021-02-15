using Core.Common.ServiceBus.Events;
using GUI.Core.Tree.Helpers;

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
                    return;
                }
            }
        }
    }
}
