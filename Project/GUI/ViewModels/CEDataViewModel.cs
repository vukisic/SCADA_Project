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
using LiveCharts;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using System.Windows.Media;
using CE.Data;
using System.Windows.Media.Animation;
using System.ComponentModel;
using System.Windows;
using DocumentFormat.OpenXml.Office.CustomUI;
using System.Web.UI.WebControls;
using System.Windows.Controls;
using System.Windows.Input;
using Realms.Sync;
using DocumentFormat.OpenXml.Wordprocessing;

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

        private ObservableCollection<DateTime> pump1X;
        private ObservableCollection<float> pump1Y; 
        private ObservableCollection<DateTime> pump2X;
        private ObservableCollection<float> pump2Y;
        private ObservableCollection<DateTime> pump3X;
        private ObservableCollection<float> pump3Y;

        public SeriesCollection IncomeSeries { get; set; }
        public SeriesCollection FluidLevelSeries { get; set; }
        public SeriesCollection WorkingSeries1 { get; set; }
        public SeriesCollection WorkingSeries2 { get; set; }
        public SeriesCollection WorkingSeries3 { get; set; }
        public SeriesCollection FlowSeries1 { get; set; }
        public SeriesCollection FlowSeries2 { get; set; }
        public SeriesCollection FlowSeries3 { get; set; }
        public SeriesCollection PumpSeriesY { get; set; }

        public LineSeries LineSeries1 { get; set; }
        public LineSeries LineSeries2 { get; set; }
        public LineSeries LineSeries3 { get; set; }

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

        public ObservableCollection<DateTime> Pump1X
        {
            get { return pump1X; }
            set
            {
                pump1X = value;
                NotifyOfPropertyChange(() => Hours);
            }
        }

        public ObservableCollection<float> Pump1Y
        {
            get { return pump1Y; }
            set
            {
                pump1Y = value;
                NotifyOfPropertyChange(() => Hours);
            }
        }

        public ObservableCollection<DateTime> Pump2X
        {
            get { return pump2X; }
            set
            {
                pump2X = value;
                NotifyOfPropertyChange(() => Hours);
            }
        }

        public ObservableCollection<float> Pump2Y
        {
            get { return pump2Y; }
            set
            {
                pump2Y = value;
                NotifyOfPropertyChange(() => Hours);
            }
        }

        public ObservableCollection<DateTime> Pump3X
        {
            get { return pump3X; }
            set
            {
                pump3X = value;
                NotifyOfPropertyChange(() => Hours);
            }
        }

        public ObservableCollection<float> Pump3Y
        {
            get { return pump3Y; }
            set
            {
                pump3Y = value;
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
            Pump1X = new ObservableCollection<DateTime>();
            Pump1Y = new ObservableCollection<float>();
            Pump2X = new ObservableCollection<DateTime>();
            Pump2Y = new ObservableCollection<float>();
            Pump3X = new ObservableCollection<DateTime>();
            Pump3Y = new ObservableCollection<float>();

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

            DrawCharts();
            DrawPumpsValuesChart();
        }
        public void ChangeVisibility1()
        {
            LineSeries1.Visibility = LineSeries1.Visibility == Visibility.Visible
                ? Visibility.Hidden
                : Visibility.Visible;
        }

        public void ChangeVisibility2()
        {
            LineSeries2.Visibility = LineSeries2.Visibility == Visibility.Visible
                ? Visibility.Hidden
                : Visibility.Visible;
        }

        public void ChangeVisibility3()
        {
            LineSeries3.Visibility = LineSeries3.Visibility == Visibility.Visible
                ? Visibility.Hidden
                : Visibility.Visible;
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

                AddGraphicValuesToIncome();
                DrawCharts();
            });
        }

        private void AddGraphicValuesToIncome()
        {
            if (Income.Count > 0)
            {
                ObservableCollection<double> newList = new ObservableCollection<double>();

                for (int i = 0; i < Income.Count - 1; i++)
                {
                    newList.Add(Income[i]);
                    newList.Add(Income[i] - (Income[i] - Income[i + 1]) / 3);
                    newList.Add(Income[i] - 2 * (Income[i] - Income[i + 1]) / 3);
                }
                newList.Add(Income[Income.Count - 1]);

                Income = newList;
            }
        }

        internal void UpdatePumpsValues(object sender, CeGraphicalEvent e)
        {
            App.Current.Dispatcher.Invoke((System.Action)delegate
            {
                Pump1X = new ObservableCollection<DateTime>();
                Pump1Y = new ObservableCollection<float>();

                foreach(var item in e.PumpsValues.Pump1.XAxes)
                {
                    Pump1X.Add(item);
                }
                foreach (var item in e.PumpsValues.Pump1.YAxes)
                {
                    Pump1Y.Add(item);
                }

                Pump2X = new ObservableCollection<DateTime>();
                Pump2Y = new ObservableCollection<float>();

                foreach (var item in e.PumpsValues.Pump2.XAxes)
                {
                    Pump2X.Add(item);
                }
                foreach (var item in e.PumpsValues.Pump2.YAxes)
                {
                    Pump2Y.Add(item);
                }

                Pump3X = new ObservableCollection<DateTime>();
                Pump3Y = new ObservableCollection<float>();

                foreach (var item in e.PumpsValues.Pump3.XAxes)
                {
                    Pump3X.Add(item);
                }
                foreach (var item in e.PumpsValues.Pump3.YAxes)
                {
                    Pump3Y.Add(item);
                }

                DrawPumpsValuesChart();
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

        private void DrawCharts()
        {
            App.Current.Dispatcher.Invoke((System.Action)delegate
            {
                IncomeSeries = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Income",
                        Values = Income.AsChartValues(),
                        Stroke = Brushes.Yellow,
                    },
                };

                FluidLevelSeries = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Fluid level",
                        Values = FluidLevel.AsChartValues(),
                        Stroke = Brushes.Red
                    },
                };

                if (Hours.Count > 0)
                {
                    WorkingSeries1 = new SeriesCollection
                    {
                        new LineSeries
                        {
                            Title = "Working time",
                            Values = Hours[0].Hours.AsChartValues(),
                            Stroke = Brushes.Green
                        },
                    };
                }

                if (Hours.Count > 1)
                {
                    WorkingSeries2 = new SeriesCollection
                    {
                        new LineSeries
                        {
                            Title = "Working time",
                            Values = Hours[1].Hours.AsChartValues(),
                            Stroke = Brushes.Green
                        },
                    };
                }

                if (Hours.Count > 2)
                {
                    WorkingSeries3 = new SeriesCollection
                    {
                        new LineSeries
                        {
                            Title = "WorkingTime",
                            Values = Hours[2].Hours.AsChartValues(),
                            Stroke = Brushes.Green
                        },
                    };
                }

                if (Flows.Count > 0)
                {
                    FlowSeries1 = new SeriesCollection
                    {
                        new LineSeries
                        {
                            Title = "Flow",
                            Values = Flows[0].Flows.AsChartValues(),
                            Stroke = Brushes.Orange
                        },
                    };
                }

                if (Flows.Count > 1)
                {
                    FlowSeries2 = new SeriesCollection
                    {
                        new LineSeries
                        {
                            Title = "Flow",
                            Values = Flows[1].Flows.AsChartValues(),
                            Stroke = Brushes.Orange
                        },
                    };
                }

                if (Flows.Count > 2)
                {
                    FlowSeries3 = new SeriesCollection
                    {
                        new LineSeries
                        {
                            Title = "Flow",
                            Values = Flows[2].Flows.AsChartValues(),
                            Stroke = Brushes.Orange
                        },
                    };
                }
            });
        }

        private void DrawPumpsValuesChart()
        {
            App.Current.Dispatcher.Invoke((System.Action)delegate
            {
                LineSeries1 = new LineSeries
                {
                    Title = "Pump1",
                    Values = Pump1Y.AsChartValues(),
                    //Values = new ChartValues<float> { 33, 56, 3, 99 },
                    Stroke = Brushes.Blue,
                };

                LineSeries2 = new LineSeries
                {
                    Title = "Pump2",
                    Values = Pump2Y.AsChartValues(),
                    //Values = new ChartValues<float> { 44, 22, 38, 99 },
                    Stroke = Brushes.Red,
                };

                LineSeries3 = new LineSeries
                {
                    Title = "Pump3",
                    Values = Pump3Y.AsChartValues(),
                    //Values = new ChartValues<float> { 77, 24, 51, 45 },
                    Stroke = Brushes.Green,
                };

                PumpSeriesY = new SeriesCollection();
                PumpSeriesY.Add(LineSeries1);
                PumpSeriesY.Add(LineSeries2);
                PumpSeriesY.Add(LineSeries3);

            });
        }
    }
}
