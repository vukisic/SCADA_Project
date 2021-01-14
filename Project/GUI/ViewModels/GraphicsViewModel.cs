using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Core.Common.ServiceBus.Commands;

namespace GUI.ViewModels
{
    public class GraphicsViewModel : Screen
    {
        internal void Update(object sender, ModelUpdateCommand e)
        {
            App.Current.Dispatcher.Invoke((System.Action)delegate
            {
                // Update code
            });
        }
    }
}
