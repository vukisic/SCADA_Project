using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;

namespace GUI.Services
{
    public class NotificationService : INotificationService
    {
        private readonly Notifier notifier;
        public NotificationService()
        {
            notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: Application.Current.MainWindow,
                    corner: Corner.TopRight,
                    offsetX: 10,
                    offsetY: 10);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(3),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(3));

                cfg.Dispatcher = Application.Current.Dispatcher;
            });
        }

        public void ShowNotification(string title, string message, NotificationType type)
        {
            switch (type)
            {
                /*case NotificationType.Error: notifier.ShowError($"{title}\n{message}"); break;
                case NotificationType.Information: notifier.ShowInformation($"{title}\n{message}"); break;
                case NotificationType.Success: notifier.ShowSuccess($"{title}\n{message}"); break;
                case NotificationType.Warning: notifier.ShowWarning($"{title}\n{message}"); break;*/
            }
        }
    }
}
