using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.ComponentModel;
using System.Diagnostics;
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

using MineStarCraft_Launcher.Helpers;
using WpfAnimatedGif;

namespace MineStarCraft_Launcher
{
    /// <summary>
    /// Lógica de interacción para Window1.xaml
    /// </summary>
    public partial class DownloadManagerView : Window
    {
        DownloadManager download;
        AuditSystem audit;

        public DownloadManagerView()
        {
            InitializeComponent();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            audit = new AuditSystem(Application.Current.MainWindow);

            download = new DownloadManager("forge");
            download.client.DownloadProgressChanged += Client_DownloadProgressChanged;
            download.ifDownloadNotOcurred += Client_DownloadFileCompleted;
            download.client.DownloadFileCompleted += new AsyncCompletedEventHandler(Client_DownloadFileCompleted);
            

            bool IsConnected = false;
            try 
            {
                IsConnected = ConnectionChecker.check();

            } catch ( PingException ex ) { audit.error("No se pudo determinar el estado de la conexion", ex); }

            if (IsConnected)
            {
                download.StartForge();
            }
            else
            {
                downloadStatusMsg.Text = "No hay conexion a la red.";

                var wifiImage = new BitmapImage();
                wifiImage.BeginInit();
                wifiImage.UriSource = new Uri("../Assets/App/svg/wifi.gif", UriKind.Relative);
                wifiImage.EndInit();

                ImageBehavior.SetAnimatedSource(gear, wifiImage);

                MessageBoxResult result = MessageBox.Show("Uups! No hay conexion con la red: \n - Conectese a la red. \n - Reinicia el Launcher",
                    "Error de red", MessageBoxButton.OK, MessageBoxImage.Error);

                finnishManager.Content = "Cerrar";
                finnishManager.IsEnabled = true;
            }
        }

        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (!e.Cancelled || e.Error != null)
            {
                downloadStatusMsg.Text = "Instalando la version de Forge 1.12.2";
                downloadProgress.Value = 100.0;

                ForgeGuide forgeWindow = new ForgeGuide(download.finalFilename);
                forgeWindow.Owner = this;
                forgeWindow.Closed += ForgeWindow_Closed;

                forgeWindow.ShowDialog();
            }
        }

        private void Client_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            downloadProgress.Value = e.ProgressPercentage;
        }

        private void ForgeWindow_Closed(object sender, EventArgs e)
        {
            ImageAnimationController gearController = ImageBehavior.GetAnimationController(gear);
            gearController.Pause();

            downloadStatusMsg.Text = "Forge 1.12.2 ha sido instalado";
            finnishManager.Content = "Cerrar";
            finnishManager.IsEnabled = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (download.client.IsBusy)
                e.Cancel = true;
        }

        private void finnishManager_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
