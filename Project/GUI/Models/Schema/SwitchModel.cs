using System.Windows.Input;
using Caliburn.Micro;
using Core.Common.ServiceBus.Dtos;

namespace GUI.Models.Schema
{
    public class SwitchModel : PropertyChangedBase, ISchemaModel
    {
        public SwitchModel()
        {
        }

        public SwitchModel(IIdentifiedObject item) : this()
        {
            Description = item.Description;
            GID = item.GID;
            MRID = item.MRID;
            Name = item.Name;
        }

        public string Description { get; set; }
        public long GID { get; set; }
        public string MRID { get; set; }
        public string Name { get; set; }

        private MeasurementModel measurement;

        public MeasurementModel Measurement
        {
            get { return measurement; }
            set
            {
                measurement = value;
                NotifyOfPropertyChange(() => Measurement);
            }
        }

        public void UpdateMeasurements(MeasurementModel newMeasurement)
        {
            measurement = newMeasurement;
        }
    }
}
