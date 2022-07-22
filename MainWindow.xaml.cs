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
            modRenderer.ModRenderProcess();

            if (MinecraftVersionChecker.checkForge())
            {
                MessageBoxResult result = MessageBox.Show("No se ha detectado Forge para Minecraft 1.12. \n¿Quieres descargarla?", 
                    "Compatibilidad de Forge", MessageBoxButton.YesNo);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        Window1 window1 = new Window1();
                        window1.Owner = this;
                        window1.ShowDialog();
                        break;
                }
            }
        }

        private void Folder_Mod_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(modDirectory))
            {
                Process.Start(modDirectory);
            }
            else
            {
                audit.error("La carpeta de \"mods\" no existe. ¿Has instalado forge?");
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
        }
    }

    
}
