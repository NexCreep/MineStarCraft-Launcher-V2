using System;
using System.IO;
using System.IO.Compression;
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
using System.Windows.Navigation;
using System.Windows.Threading;
using System.Windows.Shapes;
using System.Diagnostics;

using MineStarCraft_Launcher.Helpers;
using MineStarCraft_Launcher.ViewRenderer;
using MineStarCraft_Launcher.Models;
using System.ComponentModel;
using WpfAnimatedGif;

namespace MineStarCraft_Launcher
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        AuditSystem audit;
        string modDirectory;

        ModRenderer modRenderer;

        string actualServerImageUri;

        public MainWindow()
        {

            InitializeComponent();

            modDirectory = string.Format(@"{0}/.minecraft/mods", Environment.GetEnvironmentVariable("appdata"));

            audit = new AuditSystem(Application.Current.MainWindow);
            modRenderer = new ModRenderer(Application.Current.MainWindow, modDirectory, audit);

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                versionText.Text = $"{SettingsDB.getShortVersion()} ({SettingsDB.getLongVersion()})";
                launcherTypeText.Text = $"Launcher type: {SettingsDB.getLauncherMode()}";

                modRenderer.ModRenderProcess();

                if (SettingsDB.getLauncherMode().ToLower() == "none")
                    OpenLauncherSelector();

                ServerStatusChecker serverStatus = new ServerStatusChecker();

                DispatcherTimer timer = new DispatcherTimer();
                timer.Tick += new EventHandler(Timer_Tick);
                timer.Interval = new TimeSpan(0, 0, 5);
                timer.Start();

#if DEBUG
                DebugWindow debugWindow = new DebugWindow();
                debugWindow.Show(); 
#endif
            }
            catch (Exception except)
            {
                audit.error(except.Message, except);
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            try
            {
                if (!MinecraftVersionChecker.checkForge())
                {
                    MessageBoxResult result = MessageBox.Show("No se ha detectado Forge para Minecraft 1.12. \n¿Quieres descargarla?",
                        "Compatibilidad de Forge", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                            DownloadManagerView window1 = new DownloadManagerView();
                            window1.Owner = this;
                            window1.ShowDialog();
                            break;
                    }
                }

                Pages.ModManager modManager = new Pages.ModManager();
                modManager.modInstallationFinnished += ModManager_modInstallationFinnished;
                frameModManager.Content = modManager;
            }
            catch (Exception except)
            {
                audit.error(except.Message, except);
            }
        }

        private void ModManager_modInstallationFinnished(object sender, EventArgs e)
        {
            modRenderer.ModRenderProcess();
        }

        private async void Timer_Tick(object sender, EventArgs e)
        {
            if (!ConnectionChecker.check())
            {
                if (actualServerImageUri != "../Assets/App/svg/wifi.gif")
                {
                    BitmapImage onlineGif = new BitmapImage();
                    onlineGif.BeginInit();
                    onlineGif.UriSource = new Uri("../Assets/App/svg/wifi.gif", UriKind.Relative);
                    onlineGif.EndInit();

                    ImageBehavior.SetAnimatedSource(ServerStatusGif, onlineGif);

                    actualServerImageUri = "../Assets/App/svg/wifi.gif";
                }

                ServerStatusResult.Text = "Not Connection";
                ServerStatusResult.Foreground = Brushes.Yellow;

                return;
            }

            ServerStatusData statusData = await ServerStatusChecker.StartCheck();
            
            if (statusData.Lantency >= 0)
            {
                if (actualServerImageUri != "../Assets/App/svg/server-running.gif")
                {
                    BitmapImage onlineGif = new BitmapImage();
                    onlineGif.BeginInit();
                    onlineGif.UriSource = new Uri("../Assets/App/svg/server-running.gif", UriKind.Relative);
                    onlineGif.EndInit();

                    ImageBehavior.SetAnimatedSource(ServerStatusGif, onlineGif);

                    actualServerImageUri = "../Assets/App/svg/server-running.gif";
                }

                ServerStatusResult.Text = "Online";
                ServerStatusResult.Foreground = Brushes.LightGreen;

                ServerStatusLatency.Text = $"Latencia: {statusData.Lantency} ms";
                ServerStatusVersion.Text = $"Versión: {statusData.Version}";
                ServerStatusPlayers.Text = $"Jugadores: {statusData.ActualPlayers}/{statusData.MaxPlayers}";
            }
            else
            {
                if (actualServerImageUri != "../Assets/App/svg/server-down.gif")
                {
                    BitmapImage onlineGif = new BitmapImage();
                    onlineGif.BeginInit();
                    onlineGif.UriSource = new Uri("../Assets/App/svg/server-down.gif", UriKind.Relative);
                    onlineGif.EndInit();

                    ImageBehavior.SetAnimatedSource(ServerStatusGif, onlineGif);

                    actualServerImageUri = "../Assets/App/svg/server-down.gif";
                }

                ServerStatusResult.Text = "Offline";
                ServerStatusResult.Foreground = Brushes.Red;

                ServerStatusLatency.Text = $"Latencia: ?";
                ServerStatusVersion.Text = $"Versión: ?";
                ServerStatusPlayers.Text = $"Jugadores: ?";
            }
        }

        private void OpenLauncherSelector()
        {
            Pages.LauncherSelector launcherSelector = new Pages.LauncherSelector();
            launcherSelector.FinnishSelection += LauncherSelector_FinnishSelection;
            SelectorFrame.Content = launcherSelector;
        }

        private void LauncherSelector_FinnishSelection(object sender, EventArgs e)
        {
            SelectorFrame.Content = null;
            launcherTypeText.Text = $"Launcher type: {SettingsDB.getLauncherMode()}";
        }

        private void Folder_Mod_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(modDirectory))
            {
                Process.Start(modDirectory);
            }
            else
            {
                audit.warm("La carpeta de \"mods\" no existe. ¿Has instalado forge?");
            }
        }

        private void StartLauncherButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo("explorer.exe",
                        @"shell:appsFolder\Microsoft.4297127D64EC6_8wekyb3d8bbwe!Minecraft"
                );
    
                if (SettingsDB.getLauncherMode().ToLower() == "tlauncher")
                {
                    if (File.Exists($"{ Environment.GetEnvironmentVariable("appdata") }\\.minecraft\\TLauncher.exe"))
                    {
                        startInfo = new ProcessStartInfo("explorer.exe",
                        $"{ Environment.GetEnvironmentVariable("appdata") }\\.minecraft\\TLauncher.exe");
                    }
                    else
                        throw new Win32Exception("Tlauncher is not installed");
                }
                
                Process launcherProcc = Process.Start(startInfo);
                Close();
                
            }
            catch( Win32Exception ex )
            {
                audit.error($"No se pudo iniciar el launcher", ex);
            }
        }

        private void ChangeLauncherButton_Click(object sender, RoutedEventArgs e)
        {
            OpenLauncherSelector();
        }

    }

    
}
