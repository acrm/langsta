using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LangStat.Client.LanguageSourcesComponent
{
    public partial class LanguageSourcesView : UserControl
    {
        public LanguageSourcesView()
        {
            InitializeComponent();
        }

        public LanguageSourcesViewModel Model
        {
            get { return DataContext as LanguageSourcesViewModel; }
            set { DataContext = value; }
        }
    }
}
