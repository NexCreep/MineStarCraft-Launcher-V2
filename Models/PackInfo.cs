using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineStarCraft_Launcher.Models
{
    public class PackInfo
    {
        public string Version { get; set; }
        public string UploadedAt { get; set; }
        public IList<string> SetConfig { get; set; }
        public IList<string> NewMods { get; set; }
        public IList<string> RemovedMods { get; set; }
    }
}
