using MineStarCraft_Launcher.Helpers;
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

using MineStarCraft_Launcher.Models;
using System.Threading;

namespace MineStarCraft_Launcher.Views
{
    /// <summary>
    /// Lógica de interacción para ModPackDownloadWindow.xaml
    /// </summary>
    public partial class ModPackDownloadWindow : Window
    {
        private PackInfo packInfo;

        public event EventHandler onWindowClosed;

        public ModPackDownloadWindow(PackInfo packInfo)
        {
            this.packInfo = packInfo;
            InitializeComponent();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            DownloadManager downloadManager = new DownloadManager("mods");
            downloadManager.modActionCompleted += DownloadManager_modActionCompleted;
            downloadManager.StartModPack(packInfo);
            ModAction.Text = $"Se ha completado la actualización {packInfo.Version}";
        }

        private void DownloadManager_modActionCompleted(object sender, ModStatusArgs e)
        {
            if (!e.completed)
            {
                if (e.action.ToLower() == "deletion")
                    ModAction.Text = $"Desinstalando {e.modData.modName}";
                else if (e.action.ToLower() == "addition")
                    ModAction.Text = $"Instalando {e.modData.modName}";

                ModActionBar.Value = 0;

                return;
            }

            ModActionBar.Value = 100;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            onWindowClosed.Invoke(this, EventArgs.Empty);
        }
    }
}
