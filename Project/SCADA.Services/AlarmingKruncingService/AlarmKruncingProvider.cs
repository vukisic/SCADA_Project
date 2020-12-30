using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common.DataModel;
using SCADA.Services.Common;

namespace SCADA.Services.AlarmingKruncingService
{
    public class AlarmKruncingProvider : IAlarmKruncing
    {
        public AlarmKruncingProvider() { }

        public List<BasePoint> Check(List<BasePoint> points)
        {
            foreach (var point in points)
            {
                if (point.RegisterType == RegisterType.ANALOG_INPUT || point.RegisterType == RegisterType.ANALOG_OUTPUT)
                    ProccessAnalog(point as AnalogPoint);
                else
                    ProccessDigital(point as DiscretePoint);
            }

            return points;
        }

        public void ProccessAnalog(AnalogPoint point) {

            point.TimeStamp = DateTime.Now.ToString();
            ProccessAnalogAlarm(point);
            ProccessEGUValue(point);

        }

        public void ProccessAnalogAlarm(AnalogPoint point)
        {

            if (point.Value > point.MaxValue)
                point.Alarm = AlarmType.HIGH_ALARM;
            else if (point.Value < point.MinValue)
                point.Alarm = AlarmType.LOW_ALARM;
            else
                point.Alarm = AlarmType.NO_ALARM;

        }

        public void ProccessDigital(DiscretePoint point) {

            point.TimeStamp = DateTime.Now.ToString();
            ProccessDigitalAlarm(point);

        }

        public void ProccessDigitalAlarm(DiscretePoint point) {

            if (point.Value != point.NormalValue)
                point.Alarm = AlarmType.ABNORMAL_VALUE;
            else
                point.Alarm = AlarmType.NO_ALARM;

        }

        public void ProccessEGUValue(AnalogPoint point) {

            point.Value = (point.Value * float.Parse(ConfigurationManager.AppSettings["Analog_Scale"])) +
                float.Parse(ConfigurationManager.AppSettings["Analog_Deviation"]); 

        }
    }
}
