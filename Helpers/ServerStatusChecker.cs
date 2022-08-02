using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MineStatLib;

namespace MineStarCraft_Launcher.Helpers
{

    class ServerStatusData
    {
        public long Lantency { get; set; }
        public string Version { get; set; }
        public int ActualPlayers { get; set; }
        public int MaxPlayers { get; set; }
    }

    class ServerStatusChecker
    {
        public static ServerStatusData StartCheck()
        {
            string minecraftServer = Properties.Settings.Default.minecraftServer;
            string[] addressSubstract = minecraftServer.Split(':');


            MineStat stats = new MineStat(addressSubstract[0], ushort.Parse(addressSubstract[1]), 1);
            if (stats.ServerUp)
            {
                return new ServerStatusData {
                    Lantency = stats.Latency,
                    Version = stats.Version,
                    ActualPlayers = stats.CurrentPlayersInt,
                    MaxPlayers = stats.MaximumPlayersInt,
                };
                
            }

            return new ServerStatusData
            {
                Lantency = -1
            };
        }

    }
}
