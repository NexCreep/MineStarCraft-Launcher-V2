using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace MineStarCraft_Launcher
{
    /// <summary>
    /// Lógica de interacción para DebugWindow.xaml
    /// </summary>

    public class DataObject
    {
        public string A { get; set; }
        public string B { get; set; }
    }

    public partial class DebugWindow : Window
    {
        public DebugWindow()
        {
            InitializeComponent();
        }

        private void setData()
        {
            ObservableCollection<DataObject> list = new ObservableCollection<DataObject>();
            list.Add(new DataObject { A = "Version (short)", B = Helpers.SettingsDB.getShortVersion() });
            list.Add(new DataObject { A = "Version (long)", B = Helpers.SettingsDB.getLongVersion() });
            list.Add(new DataObject { A = "Mod Pack Version", B = Helpers.SettingsDB.getActuaModPackVersion() });
            list.Add(new DataObject { A = "Launcher Type", B = Helpers.SettingsDB.getLauncherMode() });
            list.Add(new DataObject { A = "Mod Pack File Url", B = Helpers.SettingsDB.getModPackUrl() });
            list.Add(new DataObject { A = "Forge Installer Url", B = Helpers.SettingsDB.getForgeUrl() });
            list.Add(new DataObject { A = "Fisrt Run", B = Properties.Settings.Default.firstRun.ToString() });
            dataGrid1.ItemsSource = list;
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            Helpers.SettingsDB.resetSettings();
            setData();
        }

        private void Reload_Click(object sender, RoutedEventArgs e)
        {
            setData();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            setData();
        }
    }
}
