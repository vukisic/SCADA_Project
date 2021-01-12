using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Core.Common.ServiceBus.Events;
using GUI.Core;
using GUI.Models;

namespace GUI.ViewModels
{
    public class HistoryViewModel : Screen
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

        private ObservableCollection<HistoryDto> tableData;
        public ObservableCollection<HistoryDto> TableData
        {
            get { return tableData; }
            set
            {
                tableData = value;
                NotifyOfPropertyChange(() => TableData);
            }
        }



        public HistoryViewModel()
        {
            TableData = new ObservableCollection<HistoryDto>();
            foreach (var item in Data.History)
            {
                TableData.Add(item);
            }
        }

        internal void Update(object sender, HistoryUpdateEvent e)
        {
            App.Current.Dispatcher.Invoke((System.Action)delegate
            {
                TimeStamp = DateTime.Now.ToString();
                Data.History.Clear();
                TableData = new ObservableCollection<HistoryDto>();
                foreach (var item in e.History)
                {
                    TableData.Add(Mapper.Map<HistoryDto>(item));
                    Data.History.Add(Mapper.Map<HistoryDto>(item));
                }
            });
        }
    }
}
