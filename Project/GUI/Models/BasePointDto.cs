﻿using Caliburn.Micro;
using FTN.Common;
using SCADA.Common.DataModel;

namespace GUI.Models
{
    public class BasePointDto : PropertyChangedBase, IScadaPointDto
    {
        #region Fields

        private ClassType classType;
        private SignalDirection direction;
        private int index;
        private string mrid;
        private string objectMrid;
        private RegisterType registerType;
        private string timeStamp;
        private MeasurementType measurementType;
        private AlarmType alarm;
        private double commandedValue;
        private float minValue;
        private float maxValue;
        private float normalValue;
        private float value;

        #endregion Fields

        #region ctors

        public BasePointDto()
        {
        }

        public BasePointDto(IScadaPointDto dto)
        {
            ClassType = dto.ClassType;
            Direction = dto.Direction;
            Index = dto.Index;
            Mrid = dto.Mrid;
            ObjectMrid = dto.ObjectMrid;
            RegisterType = dto.RegisterType;
            TimeStamp = dto.TimeStamp;
            MeasurementType = dto.MeasurementType;
            Alarm = dto.Alarm;
            MinValue = dto.MinValue;
            MaxValue = dto.MaxValue;
            NormalValue = dto.NormalValue;
            Value = dto.Value;
        }

        #endregion ctors

        #region Properties

        public double CommandedValue
        {
            get { return this.commandedValue; }
            set
            {
                this.commandedValue = value;
                this.NotifyOfPropertyChange(() => this.CommandedValue);
            }
        }

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

        public float MinValue
        {
            get { return this.minValue; }
            set
            {
                this.minValue = value;
                this.NotifyOfPropertyChange(() => this.MinValue);
            }
        }

        public float MaxValue
        {
            get { return this.maxValue; }
            set
            {
                this.maxValue = value;
                this.NotifyOfPropertyChange(() => this.MaxValue);
            }
        }

        public float NormalValue
        {
            get { return this.normalValue; }
            set
            {
                this.normalValue = value;
                this.NotifyOfPropertyChange(() => this.NormalValue);
            }
        }

        public float Value
        {
            get { return this.value; }
            set
            {
                this.value = value;
                this.NotifyOfPropertyChange(() => this.Value);
            }
        }

        #endregion Properties
    }
}
