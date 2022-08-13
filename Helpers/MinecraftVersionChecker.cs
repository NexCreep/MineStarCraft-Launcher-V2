using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineStarCraft_Launcher.Helpers
{
    class MinecraftVersionChecker
    {
        public static bool checkForge()
        {
            string versionsDirectory = string.Format(@"{0}/.minecraft/versions", Environment.GetEnvironmentVariable("appdata"));
            if (Directory.Exists($"{versionsDirectory}/1.12.2-forge-14.23.5.2860"))
            {
                Properties.Settings.Default.isForgeInstalled = true;
                Properties.Settings.Default.Save();
                return true;
            }

            return false;
        }
    }
}
