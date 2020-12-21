using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using FTN.Common;
using GUI.Models;
using SCADA.Common.DataModel;

namespace GUI.ViewModels
{
    public class ScadaDataViewModel : Conductor<object>
    {
        private ObservableCollection<BasePointDto> _points;
        public ObservableCollection<BasePointDto> Points
        {
            get { return _points; }
            set
            {
                _points = value;
                NotifyOfPropertyChange(() => Points);
            }
        }

        public ScadaDataViewModel()
        {
            
            Points = new ObservableCollection<BasePointDto>();
            AnalogPointDto analogPoint = new AnalogPointDto()
            {
                ClassType = ClassType.CLASS_1,
                Direction = FTN.Common.SignalDirection.ReadWrite,
                RegisterType = RegisterType.ANALOG_OUTPUT,
                Index = 200,
                MaxValue = 400,
                MinValue = 100,
                MeasurementType = FTN.Common.MeasurementType.Current,
                Mrid = "mrid1",
                NormalValue = 200,
                ObjectMrid = "NotNull",
                TimeStamp = "Timestampp",
                Value = 200,
                Alarm = AlarmType.NO_ALARM
            };

            AnalogPointDto analogPoint2 = new AnalogPointDto()
            {
                ClassType = ClassType.CLASS_1,
                Direction = FTN.Common.SignalDirection.ReadWrite,
                RegisterType = RegisterType.ANALOG_INPUT,
                Index = 200,
                MaxValue = 400,
                MinValue = 100,
                MeasurementType = FTN.Common.MeasurementType.Current,
                Mrid = "mrid1",
                NormalValue = 200,
                ObjectMrid = "NotNull",
                TimeStamp = "Timestampp",
                Value = 200,
                Alarm = AlarmType.NO_ALARM
            };

            DiscretePointDto discretePoint = new DiscretePointDto()
            {
                ClassType = ClassType.CLASS_2,
                Direction = FTN.Common.SignalDirection.ReadWrite,
                RegisterType = RegisterType.BINARY_OUTPUT,
                Index = 1,
                MaxValue = 1,
                MinValue = 0,
                MeasurementType = FTN.Common.MeasurementType.Discrete,
                Mrid = "mrid2",
                NormalValue = 0,
                ObjectMrid = "NotNull",
               TimeStamp = "Timestampp",
               Value = 0,
                Alarm = AlarmType.NO_ALARM
            };
           
            Points.Add(analogPoint);
            Points.Add(analogPoint2);
            Points.Add(discretePoint);
        }
    }
}
