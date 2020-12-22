using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using GUI.Models;

namespace GUI.ViewModels
{
    public class DOMViewModel : Conductor<object>
    {
        private ObservableCollection<SwitchingEquipmentDto> tableData;
        public ObservableCollection<SwitchingEquipmentDto> TableData
        {
            get { return tableData; }
            set
            {
                tableData = value;
                NotifyOfPropertyChange(() => TableData);
            }
        }
        public DOMViewModel()
        {
            tableData = new ObservableCollection<SwitchingEquipmentDto>();

            SwitchingEquipmentDto first = new SwitchingEquipmentDto() { ManipulationConut = 3, Mrid = "id of first" };
            SwitchingEquipmentDto sec = new SwitchingEquipmentDto() { ManipulationConut = 3, Mrid = "id of sec" };

            tableData.Add(first);
            tableData.Add(sec);
        }
    }
}
