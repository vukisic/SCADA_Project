using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace GUI.Models
{
    public class SwitchingEquipmentDto : PropertyChangedBase
    {
        public SwitchingEquipmentDto() { }

        #region Fields
        private string mrid;
        private int manipulationConut;
        #endregion

        #region Properties

        public string Mrid
        {
            get { return this.mrid; }
            set
            {
                this.mrid = value;
                this.NotifyOfPropertyChange(() => this.Mrid);
            }
        }
        public int ManipulationConut
        {
            get { return this.manipulationConut; }
            set
            {
                this.manipulationConut = value;
                this.NotifyOfPropertyChange(() => this.ManipulationConut);
            }
        }
        #endregion
    }
}
