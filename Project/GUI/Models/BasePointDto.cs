using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Core.Common.ServiceBus.Events;
using FTN.Common;
using GUI.Command;
using GUI.ServiceBus;
using NServiceBus;
using SCADA.Common.DataModel;

namespace GUI.Models
{
    public class BasePointDto : PropertyChangedBase
    {
        #region Fields

        private IEndpointInstance instance;
        public MyICommand WriteCommand { get; set; }
        public MyICommand ReadCommand { get; set; }

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
        #endregion
        public BasePointDto()
        {
            WriteCommand = new MyICommand(WriteCommand_Execute, WriteCommand_CanExecute);
            ReadCommand = new MyICommand(ReadCommand_Execute);
        }
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
        #endregion

        #region Commands

        protected bool WriteCommand_CanExecute(object obj)
        {
            if (RegisterType == RegisterType.ANALOG_OUTPUT)
                return !(CommandedValue < minValue || CommandedValue > maxValue);
            else if (RegisterType == RegisterType.BINARY_OUTPUT)
                return !(CommandedValue < 0 || CommandedValue > 1);
            else
                return false;
        }

        protected void WriteCommand_Execute(object obj)
        {
            try
            {
                instance = ServiceBusStartup.StartInstance()
                   .ConfigureAwait(false)
                   .GetAwaiter()
                   .GetResult();

                ScadaCommandingEvent ev = new ScadaCommandingEvent()
                {
                    Index = (uint)(int)Index, Milliseconds = 0, RegisterType = RegisterType.ANALOG_OUTPUT, Value = (uint)(int)CommandedValue
                };

                instance.Publish(ev).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void ReadCommand_Execute(object obj)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion 
    }
}
