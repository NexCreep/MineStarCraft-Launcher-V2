﻿using System;
using System.Collections.Generic;
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
    public partial class Window1 : Window
    {
        DownloadManager download;

        public Window1()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            download = new DownloadManager("forge");
            download.client.DownloadProgressChanged += Client_DownloadProgressChanged;
            download.client.DownloadFileCompleted += new AsyncCompletedEventHandler(Client_DownloadFileCompleted);

            download.start();
        }

        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (!e.Cancelled || e.Error != null)
            {
                downloadStatusMsg.Text = "Instalando la version de Forge 1.12.2";

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
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
    }
}
