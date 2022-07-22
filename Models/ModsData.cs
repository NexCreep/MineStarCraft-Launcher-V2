using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineStarCraft_Launcher.Models
{
    class ModsData
    {
        public int modsCount { get; set; }
        public string modsTextColor { get; set; } 
        public List<ModData> modsList { get; set; }
    }
}
