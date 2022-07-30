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
using System.Windows.Shapes;
using System.Diagnostics;

using MineStarCraft_Launcher.Helpers;
using MineStarCraft_Launcher.ViewRenderer;
using MineStarCraft_Launcher.Models;
using System.ComponentModel;

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

        public MainWindow()
        {

            InitializeComponent();

            modDirectory = string.Format(@"{0}/.minecraft/mods", Environment.GetEnvironmentVariable("appdata"));

            audit = new AuditSystem(Application.Current.MainWindow);
            modRenderer = new ModRenderer(Application.Current.MainWindow, modDirectory, audit);

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            versionText.Text = $"{SettingsDB.getShortVersion()} ({SettingsDB.getLongVersion()})";
            launcherTypeText.Text = $"Launcher type: {SettingsDB.getLauncherMode()}";

            modRenderer.ModRenderProcess();

#if DEBUG
            DebugWindow debugWindow = new DebugWindow();
            debugWindow.Show();
#endif

            if (!MinecraftVersionChecker.checkForge())
            {
                MessageBoxResult result = MessageBox.Show("No se ha detectado Forge para Minecraft 1.12. \n¿Quieres descargarla?", 
                    "Compatibilidad de Forge", MessageBoxButton.YesNo, MessageBoxImage.Information);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        Window1 window1 = new Window1();
                        window1.Owner = this;
                        window1.ShowDialog();
                        break;
                }
            }

            if (SettingsDB.getLauncherMode().ToLower() == "none")
                OpenLauncherSelector();
                
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
        }
    }

    
}
