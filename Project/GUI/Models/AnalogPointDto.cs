using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.Models
{
    public class AnalogPointDto : BasePointDto
    {
        public AnalogPointDto() { }

        #region Fields

        private float minValue;
        private float maxValue;
        private float normalValue;
        private float value;

        #endregion

        #region Properties


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

        #region Command
        protected override bool WriteCommand_CanExecute(object obj)
        {
            return !(CommandedValue < 100|| CommandedValue > 400);
        }

        protected override void WriteCommand_Execute(object obj)
        {
            try
            {

            }
            catch (Exception ex)
            {

            }
        }
        #endregion
    }
}
