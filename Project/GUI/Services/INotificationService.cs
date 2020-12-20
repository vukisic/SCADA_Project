using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.Services
{
    public interface INotificationService
    {
        void ShowNotification(string title, string message, NotificationType type);
    }
}
