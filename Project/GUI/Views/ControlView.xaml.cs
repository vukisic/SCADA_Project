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
using System.Windows.Shapes;
using GUI.Models;

namespace GUI.Views
{
    /// <summary>
    /// Interaction logic for ControlView.xaml
    /// </summary>
    public partial class ControlView : Window
    {
        public ControlView()
        {
            InitializeComponent();
        }

        public ControlView(BasePointDto dataContext) : this()
        {
            this.DataContext = dataContext;
            Title = string.Format("Control Window");
        }
    }
}
