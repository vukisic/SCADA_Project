using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Caliburn.Micro;
using Core.Common.ServiceBus.Events;
using LiveCharts;
using LiveCharts.Helpers;
using LiveCharts.Wpf;

namespace GUI.ViewModels
{
    public class HistoryGraphViewModel : Screen
    {
        #region Pump1
        private ObservableCollection<DateTime> pump1x;
        private ObservableCollection<float> pump1yc;

        public SeriesCollection Pump1Y { get; set; }
        public ObservableCollection<DateTime> Pump1X
        {
            get { return pump1x; }
            set
            {
                pump1x = value;
                NotifyOfPropertyChange(() => Pump1X);
            }
        }

        public ObservableCollection<float> Pump1Yc
        {
            get { return pump1yc; }
            set
            {
                pump1yc = value;
                NotifyOfPropertyChange(() => Pump1Yc);
            }
        }
        #endregion

        #region Pump2
        private ObservableCollection<DateTime> pump2x;
        private ObservableCollection<float> pump2yc;

        public SeriesCollection Pump2Y { get; set; }
        public ObservableCollection<DateTime> Pump2X
        {
            get { return pump2x; }
            set
            {
                pump2x = value;
                NotifyOfPropertyChange(() => Pump2X);
            }
        }

        public ObservableCollection<float> Pump2Yc
        {
            get { return pump2yc; }
            set
            {
                pump2yc = value;
                NotifyOfPropertyChange(() => Pump2Yc);
            }
        }
        #endregion

        #region Pump3
        private ObservableCollection<DateTime> pump3x;
        private ObservableCollection<float> pump3yc;

        public SeriesCollection Pump3Y { get; set; }
        public ObservableCollection<DateTime> Pump3X
        {
            get { return pump3x; }
            set
            {
                pump3x = value;
                NotifyOfPropertyChange(() => Pump3X);
            }
        }

        public ObservableCollection<float> Pump3Yc
        {
            get { return pump3yc; }
            set
            {
                pump3yc = value;
                NotifyOfPropertyChange(() => Pump3Yc);
            }
        }
        #endregion

        #region Fluid
        private ObservableCollection<DateTime> fluidx;
        private ObservableCollection<float> fluidyc;

        public SeriesCollection FluidY { get; set; }
        public ObservableCollection<DateTime> FluidX
        {
            get { return fluidx; }
            set
            {
                fluidx = value;
                NotifyOfPropertyChange(() => FluidX);
            }
        }

        public ObservableCollection<float> FluidYc
        {
            get { return fluidyc; }
            set
            {
                fluidyc = value;
                NotifyOfPropertyChange(() => FluidYc);
            }
        }
        #endregion

        public HistoryGraphViewModel()
        {
            Pump1X = new ObservableCollection<DateTime>();
            Pump1Yc = new ObservableCollection<float>();
            Pump2X = new ObservableCollection<DateTime>();
            Pump2Yc = new ObservableCollection<float>();
            Pump3X = new ObservableCollection<DateTime>();
            Pump3Yc = new ObservableCollection<float>();
            FluidX = new ObservableCollection<DateTime>();
            FluidYc = new ObservableCollection<float>();

            DrawCharts();
        }

        private void DrawCharts()
        {
            App.Current.Dispatcher.Invoke((System.Action)delegate
            {
                Pump1Y = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Pump1",
                        Values = Pump1Yc.AsChartValues(),
                        Stroke = Brushes.White,
                    },
                };

                Pump2Y = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Pump2",
                        Values = Pump2Yc.AsChartValues(),
                        Stroke = Brushes.White,
                    },
                };

                Pump3Y = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Pump3",
                        Values = Pump3Yc.AsChartValues(),
                        Stroke = Brushes.White,
                    },
                };

                FluidY = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Fluid",
                        Values = FluidYc.AsChartValues(),
                        Stroke = Brushes.White,
                    },
                };
            });
        }

        internal void Update(object sender, HistoryGraphicalEvent e)
        {
            App.Current.Dispatcher.Invoke((System.Action)delegate
            {
                Pump1X = new ObservableCollection<DateTime>();
                Pump1Yc = new ObservableCollection<float>();

                foreach (var item in e.Graph.Pump1.XAxe)
                {
                    Pump1X.Add(item);
                }
                foreach (var item in e.Graph.Pump1.YAxe)
                {
                    Pump1Yc.Add(item);
                }

                Pump2X = new ObservableCollection<DateTime>();
                Pump2Yc = new ObservableCollection<float>();

                foreach (var item in e.Graph.Pump2.XAxe)
                {
                    Pump2X.Add(item);
                }
                foreach (var item in e.Graph.Pump2.YAxe)
                {
                    Pump2Yc.Add(item);
                }

                Pump3X = new ObservableCollection<DateTime>();
                Pump3Yc = new ObservableCollection<float>();

                foreach (var item in e.Graph.Pump3.XAxe)
                {
                    Pump3X.Add(item);
                }
                foreach (var item in e.Graph.Pump3.YAxe)
                {
                    Pump3Yc.Add(item);
                }

                FluidX = new ObservableCollection<DateTime>();
                FluidYc = new ObservableCollection<float>();

                foreach (var item in e.Graph.FluidLevel.XAxe)
                {
                    FluidX.Add(item);
                }
                foreach (var item in e.Graph.FluidLevel.YAxe)
                {
                    FluidYc.Add(item);
                }

                DrawCharts();
            });
        }
    }
}
