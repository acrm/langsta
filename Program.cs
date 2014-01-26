using LangStat.Client;
using LangStat.Core;
using System;
using System.Collections.Generic;
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
            var mainWindow = new MainWindow();
            var viewModel = new MainWindowViewModel(mainWindow);
            var view = new MainWindowView { Model = viewModel };

            var application = new Application(); 
            application.MainWindow = view;
            application.Run(view);
        }
    }
}
