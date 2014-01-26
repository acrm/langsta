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
    /// <summary>
    /// Interaction logic for LanguageSourcesControllerItemView.xaml
    /// </summary>
    public partial class LanguageSourcesControllerItemView : UserControl
    {
        public LanguageSourcesControllerItemView()
        {
            InitializeComponent();
        }

        public LanguageSourceViewModel Model
        {
            get { return DataContext as LanguageSourceViewModel; }
            set { DataContext = value; }
        }
    }
}
