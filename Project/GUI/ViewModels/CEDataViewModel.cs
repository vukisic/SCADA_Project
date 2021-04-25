using System;
using Caliburn.Micro;
using Core.Common.ServiceBus.Events;
using GUI.Core;
using System.Collections.ObjectModel;
using LiveCharts;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using System.Windows.Media;
using System.Windows;

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

        private ObservableCollection<string> pump1X;
        private ObservableCollection<float> pump1Y;
        private ObservableCollection<string> pump2X;
        private ObservableCollection<float> pump2Y;
        private ObservableCollection<string> pump3X;
        private ObservableCollection<float> pump3Y;

        private SeriesCollection incomeSeries;
        private SeriesCollection fluidLevelSeries;
        private SeriesCollection workingSeries1;
        private SeriesCollection workingSeries2;
        private SeriesCollection workingSeries3;
        private SeriesCollection flowSeries1;
        private SeriesCollection flowSeries2;
        private SeriesCollection flowSeries3;
        private SeriesCollection pumpSeriesY;
        private static LineSeries lineSeries1;
        private static LineSeries lineSeries2;
        private static LineSeries lineSeries3;

        public SeriesCollection IncomeSeries
        {
            get { return incomeSeries; }
            set
            {
                incomeSeries = value;
                NotifyOfPropertyChange(() => IncomeSeries);
            }
        }
        public SeriesCollection FluidLevelSeries
        {
            get { return fluidLevelSeries; }
            set
            {
                fluidLevelSeries = value;
                NotifyOfPropertyChange(() => FluidLevelSeries);
            }
        }
        public SeriesCollection WorkingSeries1
        {
            get { return workingSeries1; }
            set
            {
                workingSeries1 = value;
                NotifyOfPropertyChange(() => WorkingSeries1);
            }
        }
        public SeriesCollection WorkingSeries2
        {
            get { return workingSeries2; }
            set
            {
                workingSeries2 = value;
                NotifyOfPropertyChange(() => WorkingSeries2);
            }
        }
        public SeriesCollection WorkingSeries3
        {
            get { return workingSeries3; }
            set
            {
                workingSeries3 = value;
                NotifyOfPropertyChange(() => WorkingSeries3);
            }
        }
        public SeriesCollection FlowSeries1
        {
            get { return flowSeries1; }
            set
            {
                flowSeries1 = value;
                NotifyOfPropertyChange(() => FlowSeries1);
            }
        }
        public SeriesCollection FlowSeries2
        {
            get { return flowSeries2; }
            set
            {
                flowSeries2 = value;
                NotifyOfPropertyChange(() => FlowSeries2);
            }
        }
        public SeriesCollection FlowSeries3
        {
            get { return flowSeries3; }
            set
            {
                flowSeries3 = value;
                NotifyOfPropertyChange(() => FlowSeries3);
            }
        }
        public SeriesCollection PumpSeriesY
        {
            get { return pumpSeriesY; }
            set
            {
                pumpSeriesY = value;
                NotifyOfPropertyChange(() => PumpSeriesY);
            }
        }

        public LineSeries LineSeries1 
        {
            get { return lineSeries1; }
            set 
            { 
                lineSeries1 = value;
                NotifyOfPropertyChange(() => LineSeries1);
            } 
        }
        public LineSeries LineSeries2
        {
            get { return lineSeries2; }
            set
            {
                lineSeries2 = value;
                NotifyOfPropertyChange(() => LineSeries2);
            }
        }
        public LineSeries LineSeries3
        {
            get { return lineSeries3; }
            set
            {
                lineSeries3 = value;
                NotifyOfPropertyChange(() => LineSeries3);
            }
        }

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

        public ObservableCollection<string> Pump1X
        {
            get { return pump1X; }
            set
            {
                pump1X = value;
                NotifyOfPropertyChange(() => Pump1X);
            }
        }

        public ObservableCollection<float> Pump1Y
        {
            get { return pump1Y; }
            set
            {
                pump1Y = value;
                NotifyOfPropertyChange(() => Pump1Y);
            }
        }

        public ObservableCollection<string> Pump2X
        {
            get { return pump2X; }
            set
            {
                pump2X = value;
                NotifyOfPropertyChange(() => Pump2X);
            }
        }

        public ObservableCollection<float> Pump2Y
        {
            get { return pump2Y; }
            set
            {
                pump2Y = value;
                NotifyOfPropertyChange(() => Pump2Y);
            }
        }

        public ObservableCollection<string> Pump3X
        {
            get { return pump3X; }
            set
            {
                pump3X = value;
                NotifyOfPropertyChange(() => Pump3X);
            }
        }

        public ObservableCollection<float> Pump3Y
        {
            get { return pump3Y; }
            set
            {
                pump3Y = value;
                NotifyOfPropertyChange(() => Pump3Y);
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
            Pump1X = new ObservableCollection<string>();
            Pump1Y = new ObservableCollection<float>();
            Pump2X = new ObservableCollection<string>();
            Pump2Y = new ObservableCollection<float>();
            Pump3X = new ObservableCollection<string>();
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
            if (PumpSeriesY.Count > 0)
            {
                (PumpSeriesY[0] as LineSeries).Visibility = (PumpSeriesY[0] as LineSeries).Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            }
        }

        public void ChangeVisibility2()
        {
            if (PumpSeriesY.Count > 1)
            {
                (PumpSeriesY[1] as LineSeries).Visibility = (PumpSeriesY[1] as LineSeries).Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            }
        }

        public void ChangeVisibility3()
        {
            if (PumpSeriesY.Count > 2)
            {
                (PumpSeriesY[2] as LineSeries).Visibility = (PumpSeriesY[2] as LineSeries).Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
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

                foreach (var item in e.Times)
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

                var temp = new SeriesCollection();
                LineSeries1 = new LineSeries
                {
                    Name = "P1",
                    Title = "Pump1",
                    Values = e.PumpsValues.Pump1.YAxes.AsChartValues(),
                    Stroke = Brushes.Khaki,
                    Visibility = Visibility.Visible
                };
                temp.Add(LineSeries1);

                LineSeries2 = new LineSeries
                {
                    Name = "P2",
                    Title = "Pump2",
                    Values = e.PumpsValues.Pump2.YAxes.AsChartValues(),
                    Stroke = Brushes.DarkSalmon,
                    Visibility = Visibility.Visible
                };
                temp.Add(LineSeries2);

                LineSeries3 = new LineSeries
                {
                    Name = "P3",
                    Title = "Pump3",
                    Values = e.PumpsValues.Pump3.YAxes.AsChartValues(),
                    Stroke = Brushes.LightGreen,
                    Visibility = Visibility.Visible
                };
                temp.Add(LineSeries3);

                PumpSeriesY = temp;
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
                        Stroke = Brushes.LightGray,
                    },
                };

                FluidLevelSeries = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Fluid level",
                        Values = FluidLevel.AsChartValues(),
                        Stroke = Brushes.LightGray
                    },
                };

                if (Hours.Count > 0)
                {
                    WorkingSeries1 = new SeriesCollection
                    {
                        new LineSeries
                        {
                            Title = "Time",
                            Values = Hours[0].Hours.AsChartValues(),
                            Stroke = Brushes.LightGray
                        },
                    };
                }

                if (Hours.Count > 1)
                {
                    WorkingSeries2 = new SeriesCollection
                    {
                        new LineSeries
                        {
                            Title = "Time",
                            Values = Hours[1].Hours.AsChartValues(),
                            Stroke = Brushes.LightGray
                        },
                    };
                }

                if (Hours.Count > 2)
                {
                    WorkingSeries3 = new SeriesCollection
                    {
                        new LineSeries
                        {
                            Title = "Time",
                            Values = Hours[2].Hours.AsChartValues(),
                            Stroke = Brushes.LightGray
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
                            Stroke = Brushes.LightGray
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
                            Stroke = Brushes.LightGray
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
                            Stroke = Brushes.LightGray
                        },
                    };
                }
            });
        }

        private void DrawPumpsValuesChart()
        {
            App.Current.Dispatcher.Invoke((System.Action)delegate
            {
                var temp = new SeriesCollection();
                LineSeries1 = new LineSeries
                {
                    Name = "P1",
                    Title = "Pump1",
                    Values = Pump1Y.AsChartValues(),
                    Stroke = Brushes.Khaki,
                    Visibility = Visibility.Visible
                };
                temp.Add(LineSeries1);

                LineSeries2 = new LineSeries
                {
                    Name = "P2",
                    Title = "Pump2",
                    Values = Pump2Y.AsChartValues(),
                    Stroke = Brushes.DarkSalmon,
                    Visibility = Visibility.Visible
                };
                temp.Add(LineSeries2);

                LineSeries3 = new LineSeries
                {
                    Name = "P3",
                    Title = "Pump3",
                    Values = Pump3Y.AsChartValues(),
                    Stroke = Brushes.LightGreen,
                    Visibility = Visibility.Visible
                };
                temp.Add(LineSeries3);

                PumpSeriesY = temp;
            });
        }
    }
}
