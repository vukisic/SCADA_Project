using System;
using System.Collections.Generic;
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
using GUI.Models;
using GUI.ViewModels;

namespace GUI.Views
{
    /// <summary>
    /// Interaction logic for ScadaDataView.xaml
    /// </summary>
    public partial class ScadaDataView : UserControl
    {
        public ScadaDataView()
        {
            InitializeComponent();

        }
        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ControlView cw = new ControlView(Points.SelectedItem as BasePointDto);
            //cw.Owner = this;
            cw.ShowDialog();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            (DataContext as IDisposable).Dispose();
        }

        private void Points_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}
