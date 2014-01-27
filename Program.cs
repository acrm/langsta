using LangStat.Client;
using LangStat.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LanguageStatisticsApp
{
    public class Program
    {
        [STAThread]
        public static void Main()
        {
            LoadExtrenalLibraries();

            var mainWindow = new MainWindow();
            var viewModel = new MainWindowViewModel(mainWindow);
            var view = new MainWindowView { Model = viewModel };

            var application = new Application(); 
            application.MainWindow = view;
            application.Run(view);
        }

        private static void LoadExtrenalLibraries()
        {
            var path = "ExtrenalLibraries";
            if(!Directory.Exists(path)) return;
            var librariesPathes = Directory.GetFiles(path, "*.dll");

            foreach(var libraryPath in librariesPathes)
            {
                using (var stream = new FileStream(libraryPath, FileMode.Open))
                {
                    var bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, bytes.Length);
                    AppDomain.CurrentDomain.Load(bytes);
                }
            }
        }
    }
}
