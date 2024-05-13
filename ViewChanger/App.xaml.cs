using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SymetricBlockEncrypter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    
    private static string _rootFolder = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\..\..\..";
    public partial class App : Application
    {
    }

    void Application_Startup(object sedner, ExitEventArgs e)
    {
        ClearTmpFiles();
    }

    void Application_Exit(object sender, ExitEventArgs e)
    {
       // ClearTmpFiles();
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
