using SCADA.Common.DataModel;

namespace GUI.Models.Schema
{
    public class MeasurementModel : BasePointDto
    {
        public MeasurementModel()
        {
        }

        public MeasurementModel(IScadaPointDto dto) : base(dto)
        {
        }

        // NOTE: Feel free to extend this method if you want to bind additional data to gauges
    }
}
