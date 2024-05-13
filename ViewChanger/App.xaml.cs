using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace SymetricBlockEncrypter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    
    public partial class App : Application
    {
        private static string _rootFolder = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\..\..\..";

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ClearTmpFiles();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            ClearTmpFiles();
        }


        private void ClearTmpFiles()
        {
            try
            {
                string[] files = Directory.GetFiles(_rootFolder + @"\RuntimeResources\Images");
                foreach (string file in files)
                {
                    if (!file.EndsWith(".gitkeep"))
                    {
                        File.Delete(file);
                    }
                }
            }
            catch { }
        }
    }

}
