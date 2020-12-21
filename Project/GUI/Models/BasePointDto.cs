using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using FTN.Common;
using SCADA.Common.DataModel;

namespace GUI.Models
{
    public class BasePointDto : PropertyChangedBase
    {
        public BasePointDto() { }

        private ClassType classType;
        private SignalDirection direction;
        private int index;
        private string mrid;
        private string objectMrid;
        private RegisterType registerType;
        private string timeStamp;
        private MeasurementType measurementType;
        private AlarmType alarm;


        public ClassType ClassType
        {
            get { return this.classType; }
            set
            {
                this.classType = value;
                this.NotifyOfPropertyChange(() => this.ClassType);
            }
        }

        public SignalDirection Direction
        {
            get { return this.direction; }
            set
            {
                this.direction = value;
                this.NotifyOfPropertyChange(() => this.Direction);
            }
        }

        public int Index
        {
            get { return this.index; }
            set
            {
                this.index = value;
                this.NotifyOfPropertyChange(() => this.Index);
            }
        }

        public string Mrid
        {
            get { return this.mrid; }
            set
            {
                this.mrid = value;
                this.NotifyOfPropertyChange(() => this.Mrid);
            }
        }

        public string ObjectMrid
        {
            get { return this.objectMrid; }
            set
            {
                this.objectMrid = value;
                this.NotifyOfPropertyChange(() => this.ObjectMrid);
            }
        }

        public RegisterType RegisterType
        {
            get { return this.registerType; }
            set
            {
                this.registerType = value;
                this.NotifyOfPropertyChange(() => this.RegisterType);
            }
        }

        public string TimeStamp
        {
            get { return this.timeStamp; }
            set
            {
                this.timeStamp = value;
                this.NotifyOfPropertyChange(() => this.TimeStamp);
            }
        }
        public MeasurementType MeasurementType
        {
            get { return this.measurementType; }
            set
            {
                this.measurementType = value;
                this.NotifyOfPropertyChange(() => this.MeasurementType);
            }
        }
        public AlarmType Alarm
        {
            get { return this.alarm; }
            set
            {
                this.alarm = value;
                this.NotifyOfPropertyChange(() => this.Alarm);
            }
        }
    }
}
