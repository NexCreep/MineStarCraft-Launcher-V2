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
            if (settingsDB.actualModPackVersion.Length == 0) settingsDB.actualModPackVersion = "v0.0";
            if (settingsDB.launcherMode.Length == 0) settingsDB.launcherMode = "none";

            settingsDB.modPackUrl = "http://localhost";
            settingsDB.forgeUrl = "http://localhost";

            settingsDB.Save();
        }

        public static void resetSettings()
        {
            settingsDB.longVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            settingsDB.shortVersion = "v2.0";
            settingsDB.actualModPackVersion = "v0.0";
            settingsDB.modPackUrl = "http://localhost";
            settingsDB.forgeUrl = "http://localhost";
            settingsDB.launcherMode = "none";
            settingsDB.firstRun = true;

            settingsDB.Save();
        }

        public static string getLongVersion() { return settingsDB.longVersion; }
        public static string getShortVersion() { return settingsDB.shortVersion; }
        public static string getActuaModPackVersion() { return settingsDB.actualModPackVersion; }
        public static string getModPackUrl() { return settingsDB.modPackUrl; }
        public static string getForgeUrl() { return settingsDB.forgeUrl; }
        public static string getLauncherMode() { return settingsDB.launcherMode; }

        public static void setActuaModPackVersion(string version) { settingsDB.actualModPackVersion = version; settingsDB.Save(); }
        public static void setLauncherMode(string mode) { settingsDB.launcherMode = mode; settingsDB.Save(); }

    }
}
