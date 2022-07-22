using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MineStarCraft_Launcher.Helpers
{
    class DownloadManager
    {
        const string FORGE_URI = "https://maven.minecraftforge.net/net/minecraftforge/forge/1.12.2-14.23.5.2860/forge-1.12.2-14.23.5.2860-installer.jar";
        const string MODPACK_URI = "https://github.com/NexCreep/MineSC-mod-pack/archive/refs/heads/main.zip";

        private string uri;

        public WebClient client;
        public string finalFilename;

        public DownloadManager(string type)
        {
            client = new WebClient();
            if (type == "forge")
                uri = FORGE_URI;
            else if (type == "modpack" || type == "mods")
                uri = MODPACK_URI;

        }

        public void start() 
        {
            finalFilename = uri.Substring(uri.LastIndexOf('/')+1);
            client.DownloadFileAsync(new Uri(uri), finalFilename);
        }

    }
}
