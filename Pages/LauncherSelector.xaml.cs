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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MineStarCraft_Launcher.Pages
{
    /// <summary>
    /// Lógica de interacción para LauncherSelector.xaml
    /// </summary>
    
    public class FinnishSelectionArgs : EventArgs { }

    public partial class LauncherSelector : Page
    {
        private string launcherSelected;

        public event EventHandler FinnishSelection;
        public delegate void FinnishSelectionEvent(object sender, FinnishSelectionArgs e);

        public LauncherSelector()
        {
            InitializeComponent();
        }

        private void accept_Click(object sender, RoutedEventArgs e)
        {
            if (launcherSelected == null)
            {
                MessageBox.Show("Debes elegir un launcher", "Error en selección", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Helpers.SettingsDB.setLauncherMode(launcherSelected);
            FinnishSelection?.Invoke(this, EventArgs.Empty);
        }

        private void switchLauncher_Click(object sender, RoutedEventArgs e)
        {
            if (sender.Equals(minecraftLauncher))
            {
                launcherSelected = "Official";
                minecraftLauncher.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFBEE6FD")) ;
                tlauncherLauncher.Background = new SolidColorBrush(Color.FromArgb(0,0,0,0));
            }
            else if (sender.Equals(tlauncherLauncher))
            {
                launcherSelected = "TLauncher";
                tlauncherLauncher.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFBEE6FD"));
                minecraftLauncher.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            }


        }
    }
}
