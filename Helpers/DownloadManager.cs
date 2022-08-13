using MineStarCraft_Launcher.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MineStarCraft_Launcher.Helpers
{
    class ModStatusArgs: EventArgs
    {
        public string action { get; set; }
        public ModData modData { get; set; }
        public bool completed { get; set; } = false;
    }

    class DownloadManager
    {
        private string uri;

        public WebClient client;
        public string finalFilename;

        public event EventHandler<AsyncCompletedEventArgs> ifDownloadNotOcurred;
        public delegate void ifDownloadNotOcurredEventHandler(object sender, AsyncCompletedEventArgs e);

        public event EventHandler<ModStatusArgs> modActionCompleted;
        public delegate void modActionCompletedEventHandler(object sender, ModStatusArgs e);

        public DownloadManager(string type)
        {
            client = new WebClient();
            if (type == "forge")
                uri = SettingsDB.getForgeUrl();
            else if (type == "modpack" || type == "mods")
                uri = SettingsDB.getModPackUrl();
        }

        public void StartForge() 
        {
            finalFilename = uri.Substring(uri.LastIndexOf('/') + 1);
            if (!File.Exists($"./{finalFilename}"))
                client.DownloadFileAsync(new Uri(uri), finalFilename);
            else
            {
                AsyncCompletedEventArgs eventArgs = new AsyncCompletedEventArgs(null, false, null);
                ifDownloadNotOcurred.Invoke(this, eventArgs);
            }
                
        }

        public void StartModPack(PackInfo pack)
        {
            string modsDirectory = $@"{Environment.GetEnvironmentVariable("appdata")}/.minecraft/mods";


            foreach (string mod in pack.RemovedMods)
            {
                ModStatusArgs args = new ModStatusArgs()
                {
                    action = "deletion",
                    modData = new ModData()
                    {
                        modName = mod.Substring(0, mod.Length - 4)
                    }
                };
                modActionCompleted.Invoke(this, args);

                File.Delete($"{modsDirectory}/{mod}");

                args.completed = true;
                modActionCompleted.Invoke(this, args);


            }

            foreach (string mod in pack.NewMods)
            {

                ModStatusArgs args = new ModStatusArgs()
                {
                    action = "addition",
                    modData = new ModData()
                    {
                        modName = mod.Substring(0, mod.Length - 4)
                    }
                };
                modActionCompleted.Invoke(this, args);

                string url = $"{SettingsDB.getModPackUrl()}/{mod}?raw=true";
                string dest = $"{Environment.GetEnvironmentVariable("appdata")}/.minecraft/mods/{mod}";

                client.DownloadFile(url, dest);

                args.completed = true;
                modActionCompleted.Invoke(this, args);
            }
        }
        

        public void stop()
        {
            client.CancelAsync();
        }

        public void applyConfig()
        {

        }

    }
}
