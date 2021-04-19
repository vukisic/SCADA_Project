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
            var result = new Dictionary<Tuple<RegisterType, int>, BasePoint>();
            foreach (var point in points.Values)
            {
                if (point.RegisterType == RegisterType.ANALOG_INPUT || point.RegisterType == RegisterType.ANALOG_OUTPUT)
                {
                    var newPoint = ProccessAnalog(point as AnalogPoint);
                    result.Add(Tuple.Create(newPoint.RegisterType, newPoint.Index), newPoint);
                }
                else
                {
                    var newPoint = ProccessDigital(point as DiscretePoint);
                    result.Add(Tuple.Create(newPoint.RegisterType, newPoint.Index), newPoint);
                }
            }

            return Task.FromResult<Dictionary<Tuple<RegisterType, int>, BasePoint>>(result);
        }

        public AnalogPoint ProccessAnalog(AnalogPoint point)
        {

            point.TimeStamp = DateTime.Now.ToString();
            var newPoint = ProccessAnalogAlarm(point);
            newPoint = ProccessEGUValue(point);
            return newPoint;
        }

        public AnalogPoint ProccessAnalogAlarm(AnalogPoint point)
        {
            if (point.Value > point.MaxValue)
                point.Alarm = AlarmType.HIGH_ALARM;
            else if (point.Value < point.MinValue)
                point.Alarm = AlarmType.LOW_ALARM;
            else
                point.Alarm = AlarmType.NO_ALARM;
            return point;

        }

        public DiscretePoint ProccessDigital(DiscretePoint point)
        {

            point.TimeStamp = DateTime.Now.ToString();
            var newPoint = ProccessDigitalAlarm(point);
            return newPoint;

        }

        public DiscretePoint ProccessDigitalAlarm(DiscretePoint point)
        {

            if (point.Value != point.NormalValue)
                point.Alarm = AlarmType.ABNORMAL_VALUE;
            else
                point.Alarm = AlarmType.NO_ALARM;
            return point;

        }

        public AnalogPoint ProccessEGUValue(AnalogPoint point)
        {
            point.Value = (point.Value * _scale) +_deviation;
            return point;
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
