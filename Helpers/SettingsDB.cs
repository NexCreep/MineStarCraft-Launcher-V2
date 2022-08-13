using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MineStarCraft_Launcher.Models;
using static System.Net.Mime.MediaTypeNames;

namespace MineStarCraft_Launcher.Helpers
{
    class SettingsDB: Settings
    {
        private static Properties.Settings settingsDB;
        public SettingsDB()
        {
            settingsDB = Properties.Settings.Default;

            if (settingsDB.firstRun)
            {
                resetSettings();

                settingsDB.firstRun = false;
            }

            if (settingsDB.longVersion.Length == 0) settingsDB.longVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            if (settingsDB.shortVersion.Length == 0) settingsDB.shortVersion = "v2.0";
            if (settingsDB.launcherMode.Length == 0) settingsDB.launcherMode = "none";

            settingsDB.modPackUrl = "https://github.com/NexCreep/minesc-modpack-v2/blob/main";
            settingsDB.forgeUrl = "https://maven.minecraftforge.net/net/minecraftforge/forge/1.12.2-14.23.5.2860/forge-1.12.2-14.23.5.2860-installer.jar";

            settingsDB.Save();
        }

        public static void resetSettings()
        {
            settingsDB.longVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            settingsDB.shortVersion = "v2.0";
            settingsDB.modPackUrl = "https://github.com/NexCreep/minesc-modpack-v2/blob/main";
            settingsDB.forgeUrl = "https://maven.minecraftforge.net/net/minecraftforge/forge/1.12.2-14.23.5.2860/forge-1.12.2-14.23.5.2860-installer.jart";
            settingsDB.launcherMode = "none";
            settingsDB.firstRun = true;
            settingsDB.isForgeInstalled = false;

            settingsDB.Save();
        }

        public static string getLongVersion() { return settingsDB.longVersion; }
        public static string getShortVersion() { return settingsDB.shortVersion; }
        public static string getModPackUrl() { return settingsDB.modPackUrl; }
        public static string getForgeUrl() { return settingsDB.forgeUrl; }
        public static string getLauncherMode() { return settingsDB.launcherMode; }

        public static void setLauncherMode(string mode) { settingsDB.launcherMode = mode; settingsDB.Save(); }
        public static void setForgeInstallation(bool flag) { settingsDB.isForgeInstalled = flag; }

    }
}
