using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Core.Common.ServiceBus.Commands;
using Core.Common.ServiceBus.Events;

namespace GUI.ViewModels
{
    public class CEDataViewModel : Screen
    {
        internal void Update(object sender, CeUpdateEvent e)
        {
            App.Current.Dispatcher.Invoke((System.Action)delegate
            {
                // Update code
            });
        }
    }
}
