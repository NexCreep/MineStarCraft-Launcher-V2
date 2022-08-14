using System;
using System.Collections.Generic;
using System.IO;
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
        private DateTime actual;

        public AuditSystem() { }
        public AuditSystem(Window currentWindow)
        {
            notifier = new Notifier(cfg =>
                {
                    cfg.PositionProvider = new WindowPositionProvider(
                        parentWindow: currentWindow,
                        corner: Corner.BottomCenter,
                        offsetX: 15,
                        offsetY: 15
                        );

                    cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                        notificationLifetime: TimeSpan.FromSeconds(3),
                        maximumNotificationCount: MaximumNotificationCount.FromCount(3)
                        );

                    cfg.Dispatcher = Application.Current.Dispatcher;
                }
            );
        }

        private void SyncTime() { actual = DateTime.Now; }

        public void info(string msg)
        {
            if (notifier != null)
                notifier.ShowInformation(string.Format("[INFO] {0}", msg));
            SyncTime();
            writeLog($"[{actual:HH:mm:ss,fff}/INFO] {msg}");
        }

        public void ok(string msg)
        {
            if (notifier != null)
                notifier.ShowSuccess(string.Format("[GOOD] {0}", msg));
            SyncTime();
            writeLog($"[{actual:HH:mm:ss,fff}/GOOD] {msg}");
        }

        public void warm(string msg)
        {
            if (notifier != null)
                notifier.ShowWarning(string.Format("[WARM]: {0}", msg));
            SyncTime();
            writeLog($"[{actual:HH:mm:ss,fff}/WARM] {msg}");
        }
        public void error(string msg, Exception e)
        {
            if (notifier != null)
                notifier.ShowError(string.Format("[ERROR]: {0}", msg));
            SyncTime();
            writeLog($"[{actual:HH:mm:ss,fff}/ERROR] {msg}: {e}");
        }

        private void writeLog(string line)
        {
            if (!Directory.Exists("./log"))
                Directory.CreateDirectory("./log");

            DateTime today = DateTime.Today;
            StreamWriter writer = new StreamWriter($"./log/{today:MM-dd-yy}.log", append: true);
            writer.WriteLine(line);
            writer.Close();

        }
    }
}
