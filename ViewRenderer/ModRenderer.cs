using System;
using System.IO;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using MineStarCraft_Launcher.Helpers;
using MineStarCraft_Launcher.Models;
using System.Windows.Media;

namespace MineStarCraft_Launcher.ViewRenderer
{
    class ModRenderer
    {

        private MainWindow window;
        private List<ModData> files;
        private AuditSystem audit;
        private string modDirectory;

        public ModRenderer(Window window, string modDirectory, AuditSystem audit)
        {
            files = new List<ModData>();

            this.audit = audit;
            this.window = (MainWindow)window;
            this.modDirectory = modDirectory;
        }

        public void ModRenderProcess()
        {
            // Comprueba si el directorio de mods existe en la carpeta de %appdata%/.minecraft.
            // Si no la crea.
            if (!Directory.Exists(modDirectory))
            {
                _ = Directory.CreateDirectory(modDirectory);
                audit.warm("La carpeta \"mods\" no existe en los archivos del Minecraft. Creandola en la carpeta de juegos");
            }

            // Obtenemos todos los mods o lo que es lo mismo, los archivos con la extension JAR.
            IEnumerable<string> mods = Directory.EnumerateFiles(modDirectory, "*.jar", SearchOption.TopDirectoryOnly);
            int modCount = 0;

            // Para cada archivo de mod lo incluimos con solo el nombre del archivo sin la extension en la lista
            // que se le da al componente de lista de WPF.
            foreach (string mod in mods)
            {
                modCount += 1;
                string modFile = mod.Substring(mod.LastIndexOf((char)92) + 1);
                files.Add(new ModData() { modName = modFile.Substring(0, modFile.Length - 4) });
            }

            // Otorgamos la lista al componente de listas de WPF.
            window.modList.ItemsSource = files;

            //Actualizamos los mods que hay
            window.modCountText.Text = modCount.ToString();
            window.modCountText.Foreground = new SolidColorBrush(modCount > 0 ? Colors.LightGreen : Colors.Red);

        }

    }
}
