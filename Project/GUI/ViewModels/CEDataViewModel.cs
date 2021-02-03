using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Core.Common.ServiceBus.Commands;
using Core.Common.ServiceBus.Events;
using GUI.Core;
using System.Collections.ObjectModel;

namespace GUI.ViewModels
{
    public class CEDataViewModel : Screen
    {
        private string updateTime;
        private ObservableCollection<string> times;
        private ObservableCollection<double> income;
        private ObservableCollection<float> fluidLevel;
        private ObservableCollection<PumpsFlows> flows;
        private ObservableCollection<PumpsHours> hours;

        #region Properties

        public string UpdateTime
        {
            get { return updateTime; }
            set
            {
                updateTime = value;
                NotifyOfPropertyChange(() => UpdateTime);
            }
        }

        public ObservableCollection<string> Times
        {
            get { return times; }
            set
            {
                times = value;
                NotifyOfPropertyChange(() => Times);
            }
        }

        public ObservableCollection<double> Income
        {
            get { return income; }
            set
            {
                income = value;
                NotifyOfPropertyChange(() => Income);
            }
        }

        public ObservableCollection<float> FluidLevel
        {
            get { return fluidLevel; }
            set
            {
                fluidLevel = value;
                NotifyOfPropertyChange(() => FluidLevel);
            }
        }

        public ObservableCollection<PumpsFlows> Flows
        {
            get { return flows; }
            set
            {
                flows = value;
                NotifyOfPropertyChange(() => Flows);
            }
        }

        public ObservableCollection<PumpsHours> Hours
        {
            get { return hours; }
            set
            {
                hours = value;
                NotifyOfPropertyChange(() => Hours);
            }
        }

        #endregion

        public CEDataViewModel()
        {
            Times = new ObservableCollection<string>();
            Income = new ObservableCollection<double>();
            FluidLevel = new ObservableCollection<float>();
            Flows = new ObservableCollection<PumpsFlows>();
            Hours = new ObservableCollection<PumpsHours>();

            foreach (var item in Data.Times)
            {
                Times.Add(item);
            }
            foreach (var item in Data.Income)
            {
                Income.Add(item);
            }
            foreach (var item in Data.FluidLevel)
            {
                FluidLevel.Add(item);
            }
            foreach (var item in Data.Flows)
            {
                Flows.Add(item);
            }
            foreach (var item in Data.Hours)
            {
                Hours.Add(item);
            }
        }

        internal void Update(object sender, CeUpdateEvent e)
        {
            App.Current.Dispatcher.Invoke((System.Action)delegate
            {
                UpdateTime = DateTime.Now.ToString();
                ClearAll();
                Times = new ObservableCollection<string>();
                Income = new ObservableCollection<double>();
                FluidLevel = new ObservableCollection<float>();
                Flows = new ObservableCollection<PumpsFlows>();
                Hours = new ObservableCollection<PumpsHours>();

                foreach(var item in e.Times)
                {
                    Times.Add(item);
                    Data.Times.Add(item);
                }

                foreach (var item in e.Income)
                {
                    Income.Add(item);
                    Data.Income.Add(item);
                }

                foreach (var item in e.FluidLevel)
                {
                    FluidLevel.Add(item);
                    Data.FluidLevel.Add(item);
                }

                foreach (var item in e.Flows)
                {
                    Flows.Add(Mapper.Map<PumpsFlows>(item));
                    Data.Flows.Add(Mapper.Map<PumpsFlows>(item));
                }

                foreach (var item in e.Hours)
                {
                    Hours.Add(Mapper.Map<PumpsHours>(item));
                    Data.Hours.Add(Mapper.Map<PumpsHours>(item));
                }
            });
        }

        private void ClearAll()
        {
            Data.Times.Clear();
            Data.Income.Clear();
            Data.FluidLevel.Clear();
            Data.Flows.Clear();
            Data.Hours.Clear();
        }
    }
}
