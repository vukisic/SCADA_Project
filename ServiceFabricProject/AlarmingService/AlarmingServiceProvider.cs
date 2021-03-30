using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Contracts;
using SCADA.Common.DataModel;
using SF.Common;

namespace AlarmingService
{
    public class AlarmingServiceProvider : IAlarmingServiceAsync
    {
        private StatelessServiceContext _context;
        private float _deviation;
        private float _scale;
        public AlarmingServiceProvider(StatelessServiceContext context)
        {
            _context = context;
        }

        public Task<Dictionary<Tuple<RegisterType, int>, BasePoint>> Check(Dictionary<Tuple<RegisterType, int>, BasePoint> points)
        {
            PrepareParams();
            foreach (var point in points.Values)
            {
                if (point.RegisterType == RegisterType.ANALOG_INPUT || point.RegisterType == RegisterType.ANALOG_OUTPUT)
                    ProccessAnalog(point as AnalogPoint);
                else
                    ProccessDigital(point as DiscretePoint);
            }

            return Task.FromResult<Dictionary<Tuple<RegisterType, int>, BasePoint>>(points);
        }

        public void ProccessAnalog(AnalogPoint point)
        {

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

        public void ProccessDigital(DiscretePoint point)
        {

            point.TimeStamp = DateTime.Now.ToString();
            ProccessDigitalAlarm(point);

        }

        public void ProccessDigitalAlarm(DiscretePoint point)
        {

            if (point.Value != point.NormalValue)
                point.Alarm = AlarmType.ABNORMAL_VALUE;
            else
                point.Alarm = AlarmType.NO_ALARM;

        }

        public void ProccessEGUValue(AnalogPoint point)
        {
            point.Value = (point.Value * _scale) +_deviation;
        }

        private void PrepareParams()
        {
            var deviation = ConfigurationReader.ReadValue(_context, "Settings", "Deviation");
            var scale = ConfigurationReader.ReadValue(_context, "Settings", "Scale");
            _deviation = deviation == null ? 0 : float.Parse(deviation);
            _scale = scale == null ? 1 : float.Parse(scale);
        }
    }
}
