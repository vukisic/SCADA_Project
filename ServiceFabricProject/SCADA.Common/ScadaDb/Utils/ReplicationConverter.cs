using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common.DataModel;
using SCADA.Common.Models;

namespace SCADA.Common.ScadaDb.Utils
{
    public class ReplicationConverter
    {
        public Dictionary<Tuple<RegisterType, int>, BasePoint> Convert (List<PointDbModel> result)
        {
            var dict = new Dictionary<Tuple<RegisterType, int>, BasePoint>();
            foreach (var item in result)
            {
                if (item.RegisterType == RegisterType.ANALOG_INPUT || item.RegisterType == RegisterType.ANALOG_OUTPUT)
                {
                    var point = new AnalogPoint()
                    {
                        Alarm = item.Alarm,
                        ClassType = item.ClassType,
                        Direction = item.Direction,
                        Index = item.Index,
                        MaxValue = item.MaxValue,
                        MeasurementType = item.MeasurementType,
                        MinValue = item.MinValue,
                        NormalValue = item.NormalValue,
                        Mrid = item.Mrid,
                        ObjectMrid = item.ObjectMrid,
                        RegisterType = item.RegisterType,
                        TimeStamp = item.TimeStamp,
                        Value = item.Value
                    };
                    dict.Add(Tuple.Create(point.RegisterType, point.Index), point);
                }
                else
                {
                    var point = new DiscretePoint()
                    {
                        Alarm = item.Alarm,
                        ClassType = item.ClassType,
                        Direction = item.Direction,
                        Index = item.Index,
                        MaxValue = (int)item.MaxValue,
                        MeasurementType = item.MeasurementType,
                        MinValue = (int)item.MinValue,
                        NormalValue = (int)item.NormalValue,
                        Mrid = item.Mrid,
                        ObjectMrid = item.ObjectMrid,
                        RegisterType = item.RegisterType,
                        TimeStamp = item.TimeStamp,
                        Value = (int)item.Value
                    };
                    dict.Add(Tuple.Create(point.RegisterType, point.Index), point);
                }
            }
            return dict;
        }

        public List<PointDbModel> ConvertForDb(List<BasePoint> points)
        {
            var list = new List<PointDbModel>();
            int counter = 0;
            foreach (var item in points)
            {
                if(item.RegisterType == RegisterType.ANALOG_INPUT || item.RegisterType == RegisterType.ANALOG_OUTPUT)
                {
                    var analogPoint = item as AnalogPoint;
                    var point = new PointDbModel()
                    {
                        Id = counter,
                        Alarm = analogPoint.Alarm,
                        ClassType = analogPoint.ClassType,
                        Direction = analogPoint.Direction,
                        Index = analogPoint.Index,
                        MaxValue = analogPoint.MaxValue,
                        MeasurementType = analogPoint.MeasurementType,
                        MinValue = analogPoint.MinValue,
                        NormalValue = analogPoint.NormalValue,
                        Mrid = analogPoint.Mrid,
                        ObjectMrid = analogPoint.ObjectMrid,
                        RegisterType = analogPoint.RegisterType,
                        TimeStamp = analogPoint.TimeStamp,
                        Value = analogPoint.Value
                    };
                    list.Add(point);
                }
                else
                {
                    var discretePoint = item as DiscretePoint;
                    var point = new PointDbModel()
                    {
                        Id = counter,
                        Alarm = discretePoint.Alarm,
                        ClassType = discretePoint.ClassType,
                        Direction = discretePoint.Direction,
                        Index = discretePoint.Index,
                        MaxValue = discretePoint.MaxValue,
                        MeasurementType = discretePoint.MeasurementType,
                        MinValue = discretePoint.MinValue,
                        NormalValue = discretePoint.NormalValue,
                        Mrid = discretePoint.Mrid,
                        ObjectMrid = discretePoint.ObjectMrid,
                        RegisterType = discretePoint.RegisterType,
                        TimeStamp = discretePoint.TimeStamp,
                        Value = discretePoint.Value
                    };
                    list.Add(point);
                }
                ++counter;
            }

            return list;
        }
    }
}
