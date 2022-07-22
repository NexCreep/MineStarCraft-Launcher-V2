using System;
using System.Collections.Generic;
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

namespace MineStarCraft_Launcher
{
    /// <summary>
    /// Lógica de interacción para ForgeGuide.xaml
    /// </summary>
    public partial class ForgeGuide : Window
    {
        private string finalFileName;

        public ForgeGuide(string finalFileName)
        {
            this.finalFileName = finalFileName;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Process proc = Process.Start(
                    new ProcessStartInfo("java.exe",
                        string.Format("-jar \"{0}\"",
                            System.IO.Path.Combine(Environment.CurrentDirectory, finalFileName))
                    )
                    {
                        CreateNoWindow = true,
                        UseShellExecute = false
                    }
                );
        }
    }
}
