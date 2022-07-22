using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace MineStarCraft_Launcher.Helpers
{
    class ConnectionChecker
    {
        public static bool check()
        {
            string hostForTest = "8.8.8.8";

            Ping ping = new Ping();
            PingReply reply = ping.Send(hostForTest, 3000);

            return reply.Status == IPStatus.Success;
        }


    }
}
