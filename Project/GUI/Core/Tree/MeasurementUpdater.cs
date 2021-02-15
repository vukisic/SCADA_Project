using Core.Common.ServiceBus.Events;
using GUI.Core.Tree.Helpers;
using GUI.Models;
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
                    return;
                }

                if (nodeToUpdate.Item is TransformerModel transformerModel)
                {
                    transformerModel.UpdateMeasurements(Map(point));
                }
            }
        }

        private BasePointDto Map(ScadaPointDto scadaPoint)
        {
            return new BasePointDto
            {
                ClassType = scadaPoint.ClassType,
                Direction = scadaPoint.Direction,
                Index = scadaPoint.Index,
                Mrid = scadaPoint.Mrid,
                ObjectMrid = scadaPoint.ObjectMrid,
                RegisterType = scadaPoint.RegisterType,
                TimeStamp = scadaPoint.TimeStamp,
                MeasurementType = scadaPoint.MeasurementType,
                Alarm = scadaPoint.Alarm,
                MinValue = scadaPoint.MinValue,
                MaxValue = scadaPoint.MaxValue,
                NormalValue = scadaPoint.NormalValue,
                Value = scadaPoint.Value
            };
        }
    }
}
