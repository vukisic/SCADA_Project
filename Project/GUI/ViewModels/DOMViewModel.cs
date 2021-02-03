using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Core.Common.ServiceBus.Events;
using GUI.Models;
using GUI.Core;
namespace GUI.ViewModels
{
    public class DOMViewModel : Conductor<object>
    {
        private string timeStamp;
        public string TimeStamp
        {
            get { return timeStamp; }
            set
            {
                timeStamp = value;
                NotifyOfPropertyChange(() => TimeStamp);
            }
        }

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
            TableData = new ObservableCollection<SwitchingEquipmentDto>();
            
        }

        internal void Update(object sender, DomUpdateEvent e)
        {
            App.Current.Dispatcher.Invoke((System.Action)delegate
            {
                TimeStamp = DateTime.Now.ToString();
                TableData =  new ObservableCollection<SwitchingEquipmentDto>();
                foreach (var item in e.DomData)
                {
                    TableData.Add(Mapper.Map<SwitchingEquipmentDto>(item));
                }
            });
        }
    }
}
