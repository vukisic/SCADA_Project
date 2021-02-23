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
using CE.Common.Proxies;
using Core.Common.ServiceBus.Events;
using GUI.Models;
using GUI.ServiceBus;
using GUI.ViewModels;
using NServiceBus;

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

        private void Window_Closed(object sender, EventArgs e)
        {
            (DataContext as IDisposable).Dispose();
        }
    }
}
