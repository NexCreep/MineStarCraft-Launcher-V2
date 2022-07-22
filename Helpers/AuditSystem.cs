using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace MineStarCraft_Launcher.Helpers
{
    class AuditSystem
    {
        private Notifier notifier;

        public AuditSystem(Window currentWindow)
        {
            notifier = new Notifier(cfg =>
                {
                    cfg.PositionProvider = new WindowPositionProvider(
                        parentWindow: currentWindow,
                        corner: Corner.BottomRight,
                        offsetX: 15,
                        offsetY: 15
                        );

                    cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                        notificationLifetime: TimeSpan.FromSeconds(6),
                        maximumNotificationCount: MaximumNotificationCount.FromCount(3)
                        );

                    cfg.Dispatcher = Application.Current.Dispatcher;
                }
            );
        }

        public void info(string msg)
        {
            notifier.ShowInformation(string.Format("[?] {0}", msg));
        }

        public void ok(string msg)
        {
            notifier.ShowSuccess(string.Format("[+] {0}", msg));
        }

        public void warm(string msg)
        {
            notifier.ShowWarning(string.Format("[!]: {0}", msg));
        }
        public void error(string msg)
        {
            notifier.ShowError(string.Format("[-]: {0}", msg));
        }
    }
}
