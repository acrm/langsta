using Infrastructure.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LangStat.Client.LanguageComponent;
using LangStat.Core.Contracts;
using LangStat.Core;

namespace LangStat.Client.LanguagesTabsComponent
{
    public class LanguagesTabsItemViewModel : ViewModelBase
    {
        private readonly Language _language;
        private readonly LanguageViewModel _languageViewModel;

        public LanguagesTabsItemViewModel(Language language, StatisticsProcessor statisticsProcessor)
        {
            _language = language;

            LanguageTitle = _language.Name;
            _languageViewModel = new LanguageViewModel(_language, statisticsProcessor);
            LanguageComponent = new LanguageView { Model = _languageViewModel };
        }

        public string LanguageTitle
        {
            get { return _languageTitle; }
            set { _languageTitle = value; RaisePropertyChanged("LanguageTitle"); }

        }

        private string _languageTitle;


        public LanguageView LanguageComponent
        {
            get 
            {
                return _languageComponent; 
            }

            set 
            {
                _languageComponent = value;
                RaisePropertyChanged("LanguageComponent"); 
            }
        }

        private LanguageView _languageComponent;
    }
}
