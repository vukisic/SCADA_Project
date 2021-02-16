using FTN.Common;

namespace SCADA.Common.DataModel
{
    public interface IScadaPointDto
    {
        AlarmType Alarm { get; set; }
        ClassType ClassType { get; set; }
        SignalDirection Direction { get; set; }
        int Index { get; set; }
        float MaxValue { get; set; }
        MeasurementType MeasurementType { get; set; }
        float MinValue { get; set; }
        string Mrid { get; set; }
        float NormalValue { get; set; }
        string ObjectMrid { get; set; }
        RegisterType RegisterType { get; set; }
        string TimeStamp { get; set; }
        float Value { get; set; }
    }
}
