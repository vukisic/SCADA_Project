using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GUI.Core;
using LiveCharts;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using GUI.ViewModels;

namespace GUI.Views
{
    /// <summary>
    /// Interaction logic for CEDataView.xaml
    /// </summary>
    public partial class CEDataView : UserControl
    {
        public CEDataView()
        {
            InitializeComponent();

            LineSeries lineSeries1 = new LineSeries { Title = "Income" };
            if (Data.Income.Count > 0)
            {
                lineSeries1.Values = Data.Income.AsChartValues();
            }

            LineSeries lineSeries2 = new LineSeries { Title = "Fluid level" };
            if (Data.FluidLevel.Count > 0)
            {
                lineSeries2.Values = Data.FluidLevel.AsChartValues();
            }

            LineSeries lineSeries3 = new LineSeries { Title = "Flow 1" };
            if (Data.Flows.Count > 0)
            {
                lineSeries3.Values = Data.Flows[0].Flows.AsChartValues();
            }

            LineSeries lineSeries4 = new LineSeries { Title = "Flow 2" };
            if (Data.Flows.Count > 1)
            {
                lineSeries4.Values = Data.Flows[1].Flows.AsChartValues();
            }

            LineSeries lineSeries5 = new LineSeries { Title = "Flow 3" };
            if (Data.Flows.Count > 2)
            { 
                lineSeries5.Values = Data.Flows[2].Flows.AsChartValues();
            }

            LineSeries lineSeries6 = new LineSeries { Title = "Working time 1" };
            if (Data.Hours.Count > 0)
            {
                lineSeries6.Values = Data.Hours[0].Hours.AsChartValues();
            }

            LineSeries lineSeries7 = new LineSeries{ Title = "Working time 2" };
            if (Data.Hours.Count > 1)
            {
                lineSeries7.Values = Data.Hours[1].Hours.AsChartValues();
            }

            LineSeries lineSeries8 = new LineSeries { Title = "Working time 3" };
            if (Data.Hours.Count > 2)
            {
                lineSeries8.Values = Data.Hours[2].Hours.AsChartValues();
            }

            MyChart.Series.Add(lineSeries1);
            MyChart.Series.Add(lineSeries2);
            MyChart.Series.Add(lineSeries3);
            MyChart.Series.Add(lineSeries4);
            MyChart.Series.Add(lineSeries5);
            MyChart.Series.Add(lineSeries6);
            MyChart.Series.Add(lineSeries7);
            MyChart.Series.Add(lineSeries8);

            Axis axis = new Axis();
            axis.Title = "Time";

            if (Data.Times.Count > 0)
            {
                axis.Labels = Data.Times;
            }

            MyChart.AxisX.Add(axis);

        }
    }
}
