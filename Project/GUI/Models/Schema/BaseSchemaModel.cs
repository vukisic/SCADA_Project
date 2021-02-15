using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;
using Core.Common.ServiceBus.Dtos;

namespace GUI.Models.Schema
{
    public class BaseSchemaModel : PropertyChangedBase, IIdentifiedObject
    {
        public BaseSchemaModel(IIdentifiedObject identifiedObject = null)
        {
            Description = identifiedObject.Description;
            GID = identifiedObject.GID;
            MRID = identifiedObject.MRID;
            Name = identifiedObject.Name;
        }

        public string Description { get; set; }
        public long GID { get; set; }
        public string MRID { get; set; }
        public string Name { get; set; }

        private ObservableCollection<BasePointDto> measurements = new ObservableCollection<BasePointDto>();

        public ObservableCollection<BasePointDto> Measurements
        {
            get { return measurements; }
            set
            {
                measurements = value;
                NotifyOfPropertyChange(() => Measurements);
            }
        }

        public virtual void UpdateMeasurements(BasePointDto newMeasurement)
        {
            var oldMeasurement = measurements
                .FirstOrDefault(measurement => measurement.MeasurementType == newMeasurement.MeasurementType);

            if (oldMeasurement is null)
            {
                Measurements.Add(newMeasurement);
                return;
            }

            UpdateMeasurement(newMeasurement, oldMeasurement);
        }

        private void UpdateMeasurement(BasePointDto newMeasurement, BasePointDto oldMeasurement)
        {
            oldMeasurement.CommandedValue = newMeasurement.CommandedValue;
            oldMeasurement.ClassType = newMeasurement.ClassType;
            oldMeasurement.Direction = newMeasurement.Direction;
            oldMeasurement.Index = newMeasurement.Index;
            oldMeasurement.Mrid = newMeasurement.Mrid;
            oldMeasurement.ObjectMrid = newMeasurement.ObjectMrid;
            oldMeasurement.RegisterType = newMeasurement.RegisterType;
            oldMeasurement.TimeStamp = newMeasurement.TimeStamp;
            oldMeasurement.MeasurementType = newMeasurement.MeasurementType;
            oldMeasurement.Alarm = newMeasurement.Alarm;
            oldMeasurement.MinValue = newMeasurement.MinValue;
            oldMeasurement.MaxValue = newMeasurement.MaxValue;
            oldMeasurement.NormalValue = newMeasurement.NormalValue;
            oldMeasurement.Value = newMeasurement.Value;
            oldMeasurement.IsNotifying = newMeasurement.IsNotifying;
        }
    }
}
