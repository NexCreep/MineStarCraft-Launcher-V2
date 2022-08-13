using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using System.Windows.Shapes;

using Newtonsoft.Json;

using MineStarCraft_Launcher.Models;
using MineStarCraft_Launcher.Helpers;
using System.IO;
using MineStarCraft_Launcher.Views;
using System.IO.Compression;

namespace MineStarCraft_Launcher.Pages
{
    /// <summary>
    /// Lógica de interacción para ModManager.xaml
    /// </summary>
    public partial class ModManager : Page
    {
        public event EventHandler modInstallationFinnished;
        public delegate void modInstallationFinnishedEventHandler(object sender, EventArgs e);

        public ModManager()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (!ConnectionChecker.check())
            {
                shortInfo.Text = "Updates not available";
                iconInfo.Icon = FontAwesome.WPF.FontAwesomeIcon.PowerOff;
                iconInfo.Foreground = Brushes.Red;
                return;
            }

            if (Properties.Settings.Default.isForgeInstalled)
            {
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadStringCompleted += Wc_DownloadStringCompleted; ;
                    wc.DownloadStringAsync(new Uri(Properties.Settings.Default.packInfoUrl));
                }
            }
            else
            {
                shortInfo.Text = "Forge no esta instalado";
                iconInfo.Icon = FontAwesome.WPF.FontAwesomeIcon.Close;
                iconInfo.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            }

        }

        private void DownloadFullPack()
        {
            using (WebClient wc = new WebClient())
            {
                wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
                wc.DownloadFileAsync(
                        new Uri("https://github.com/NexCreep/minesc-modpack-v2/archive/refs/heads/main.zip"),
                        $"{Environment.GetEnvironmentVariable("appdata")}\\.minecraft\\mods\\modpack.zip"
                    );
            }
        }

        private void Wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (!e.Cancelled && e.Error == null)
            {
                if (Directory.Exists($"{Environment.GetEnvironmentVariable("appdata")}\\.minecraft\\mods\\minesc-modpack-v2-main"))
                    Directory.Delete($"{ Environment.GetEnvironmentVariable("appdata")}\\.minecraft\\mods\\minesc-modpack-v2-main", true);

                ZipFile.ExtractToDirectory(
                        $"{Environment.GetEnvironmentVariable("appdata")}\\.minecraft\\mods\\modpack.zip",
                        $"{Environment.GetEnvironmentVariable("appdata")}\\.minecraft\\mods"
                    );

                List<string> modFiles = Directory
                    .GetFiles($"{ Environment.GetEnvironmentVariable("appdata")}\\.minecraft\\mods\\minesc-modpack-v2-main", "*.*", SearchOption.TopDirectoryOnly)
                    .ToList();

                foreach (string file in modFiles)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    fileInfo.MoveTo($"{Environment.GetEnvironmentVariable("appdata")}\\.minecraft\\mods\\" + fileInfo.Name);
                }

                Directory.Delete($"{ Environment.GetEnvironmentVariable("appdata")}\\.minecraft\\mods\\minesc-modpack-v2-main", true);
                File.Delete($"{Environment.GetEnvironmentVariable("appdata")}\\.minecraft\\mods\\modpack.zip");

                using (WebClient wc = new WebClient())
                {
                    wc.DownloadStringCompleted += Wc_DownloadStringCompleted; ;
                    wc.DownloadStringAsync(new Uri(Properties.Settings.Default.packInfoUrl));
                }
                modInstallationFinnished.Invoke(this, EventArgs.Empty);
            }
        }

        private void Wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {

            if (!e.Cancelled && e.Error == null)
            {
                PackInfo packInfo = JsonConvert.DeserializeObject<PackInfo>(e.Result);

                if (packInfo.SetConfig.Count > 0)
                {
                    foreach (string config in packInfo.SetConfig)
                    {
                        using (WebClient wc = new WebClient())
                        {
                            string newConfig = wc.DownloadString($"https://raw.githubusercontent.com/NexCreep/minesc-modpack-v2/main/.config/{config}");
                            File.WriteAllText($@"{Environment.GetEnvironmentVariable("appdata")}/.minecraft/config/{config}", newConfig);
                        }
                    }
                }
                

                if (!File.Exists($"{Environment.GetEnvironmentVariable("appdata")}\\.minecraft\\mods\\packinfo.json"))
                {
                    MessageBoxResult result = MessageBox.Show($"¿Quieres descargar el paquete de mods?\nNo version >> {packInfo.Version}",
                            "Actualización disponible",
                            MessageBoxButton.YesNo, MessageBoxImage.Question
                        );

                    if (result == MessageBoxResult.Yes)
                    {
                        shortInfo.Text = "Instalando mods...";
                        iconInfo.Icon = FontAwesome.WPF.FontAwesomeIcon.Gear;
                        iconInfo.Foreground = Brushes.Brown;
                        DownloadFullPack();
                    }

                    return;
                }

                PackInfo localPackInfo = JsonConvert.DeserializeObject<PackInfo>(
                    File.ReadAllText(
                            $"{Environment.GetEnvironmentVariable("appdata")}\\.minecraft\\mods\\packinfo.json"
                        )
                );

                if (packInfo.Version != localPackInfo.Version)
                {
                    shortInfo.Text = $"Nueva actualización";
                    longInfo.Text = $"Versión {localPackInfo.Version}";
                    iconInfo.Icon = FontAwesome.WPF.FontAwesomeIcon.Warning;
                    iconInfo.Foreground = Brushes.Yellow;

                    MessageBoxResult result = MessageBox.Show(
                        $"¿Quieres actualizar a la nueva version?\n{localPackInfo.Version} >> {packInfo.Version}",
                        "Actualización disponible",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question
                    );

                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                            File.WriteAllText(
                            $"{Environment.GetEnvironmentVariable("appdata")}\\.minecraft\\mods\\packinfo.json",
                            JsonConvert.SerializeObject(packInfo)
                            );

                            ModPackDownloadWindow downloadWindow = new ModPackDownloadWindow(packInfo);
                            downloadWindow.onWindowClosed += DownloadWindow_onWindowClosed;
                            downloadWindow.Owner = Window.GetWindow(this);
                            downloadWindow.ShowDialog();
                            break;

                    }

                    return;
                }

                shortInfo.Text = $"Todo actualizado";
                longInfo.Text = $"Versión {localPackInfo.Version}";
                iconInfo.Icon = FontAwesome.WPF.FontAwesomeIcon.Check;
                iconInfo.Foreground = Brushes.LightGreen;
            }
        }

        private void DownloadWindow_onWindowClosed(object sender, EventArgs e)
        {
            using (WebClient wc = new WebClient())
            {
                wc.DownloadStringCompleted += Wc_DownloadStringCompleted; ;
                wc.DownloadStringAsync(new Uri(Properties.Settings.Default.packInfoUrl));
            }
        }
    }
}
