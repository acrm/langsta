using LangStat.Client.LanguageComponent;
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

namespace LangStat.Client.LanguageComponent
{
    public partial class LanguageView : UserControl
    {
        public LanguageView()
        {
            InitializeComponent();
        }

        public LanguageViewModel Model
        {
            get { return DataContext as LanguageViewModel; }
            set { DataContext = value; }
        }
    }
}
