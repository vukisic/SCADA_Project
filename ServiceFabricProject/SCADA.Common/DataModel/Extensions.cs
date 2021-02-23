using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTN.Common;
using SCADA.Common.Models;

namespace SCADA.Common.DataModel
{
    public static class Extensions
    {
        public static HistoryDbModel ToHistoryDbModel(this AnalogPoint point)
        {
            var model = new HistoryDbModel();
            model.ClassType = point.ClassType;
            model.Index = point.Index;
            model.MeasurementType =Enum.GetName(typeof(MeasurementType), point.MeasurementType);
            model.Mrid = point.Mrid;
            model.RegisterType = point.RegisterType;
            model.TimeStamp = point.TimeStamp;
            model.Value = point.Value;
            return model;
        }

        public static HistoryDbModel ToHistoryDbModel(this DiscretePoint point)
        {
            var model = new HistoryDbModel();
            model.ClassType = point.ClassType;
            model.Index = point.Index;
            model.MeasurementType = Enum.GetName(typeof(MeasurementType), point.MeasurementType);
            model.Mrid = point.Mrid;
            model.RegisterType = point.RegisterType;
            model.TimeStamp = point.TimeStamp;
            model.Value = point.Value;
            return model;
        }
    }
}
