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

        private AuditSystem audit;

        public ModManager()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            audit = new AuditSystem();

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
            try
            {
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
                    wc.DownloadFileAsync(
                            new Uri("https://github.com/NexCreep/minesc-modpack-v2/archive/refs/heads/main.zip"),
                            $"{Environment.GetEnvironmentVariable("appdata")}\\.minecraft\\mods\\modpack.zip"
                        );
                }
            }catch (Exception except)
            {
                audit.error(except.Message, except);
            }
        }

        private void Wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            try
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
            } catch (Exception except)
            {
                audit.error(except.Message, except);
            }
        }

        private void Wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
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
                                FileStream fs = null;

                                if (!File.Exists($@"{Environment.GetEnvironmentVariable("appdata")}/.minecraft/config/{config}"))
                                {
                                    Directory.CreateDirectory($@"{Environment.GetEnvironmentVariable("appdata")}/.minecraft/config/{config.Substring(0, config.IndexOf('/'))}");
                                    fs = File.Create($@"{Environment.GetEnvironmentVariable("appdata")}/.minecraft/config/{config}");
                                }

                                if (fs == null)
                                {
                                    fs = new FileStream($@"{Environment.GetEnvironmentVariable("appdata")}/.minecraft/config/{config}", FileMode.Create, FileAccess.Write);
                                }

                                byte[] byteArray = new UTF8Encoding(true).GetBytes(newConfig);
                                fs.Write(byteArray, 0, byteArray.Length);
                                fs.Close();

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

                        string contentMsg = $"¿Quieres actualizar a la nueva versión?\n{localPackInfo.Version} >> {packInfo.Version}\n\nSe añadirán:\n";
                        foreach (string newmod in packInfo.NewMods)
                        {
                            contentMsg += $"- {newmod}\n";
                        }
                        contentMsg += $"\nSe eliminarán:\n";
                        foreach (string oldmod in packInfo.RemovedMods)
                        {
                            contentMsg += $"- {oldmod}\n";
                        }


                        MessageBoxResult result = MessageBox.Show(
                            contentMsg,
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
            } catch (Exception except)
            {
                audit.error(except.Message, except);
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
