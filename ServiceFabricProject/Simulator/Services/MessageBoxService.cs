using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit;

namespace Simulator.Services
{
    public class MessageBoxService : IMessageService
    {
        public void ShowMessage(string message)
        {
            MessageBox.Show(message, "Simulator");
        }
    }
}
