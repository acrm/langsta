using Infrastructure.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LangStat.Core.Contracts;
using LangStat.Client.LanguageSourcesComponent;
using LangStat.Client.LanguageComponent.LanguageStatisticsComponent;
using LangStat.Core;

namespace LangStat.Client.LanguageComponent
{
    public class LanguageViewModel : ViewModelBase
    {
        private readonly Language _language;
        private readonly LanguageStatisticsViewModel _languageStatisticsViewModel;
        private readonly LanguageSourcesViewModel _languageSourcesViewModel;

        public LanguageViewModel(Language language, StatisticsProcessor statisticsProcessor)
        {
            _language = language;

            _languageStatisticsViewModel = new LanguageStatisticsViewModel(language, statisticsProcessor);
            LanguageStatisticsView = new LanguageStatisticsView { Model = _languageStatisticsViewModel };

            _languageSourcesViewModel = new LanguageSourcesViewModel(_language, _language.LanguageSourcesRepository);
            LanguageSourcesView = new LanguageSourcesView { Model = _languageSourcesViewModel };
        }

        public LanguageSourcesView LanguageSourcesView
        {
            get { return _languageSourcesView; }
            set { _languageSourcesView = value; RaisePropertyChanged("LanguageSourcesView"); }
        }

        private LanguageSourcesView _languageSourcesView;
        
        public LanguageStatisticsView LanguageStatisticsView
        {
            get { return _languageStatisticsView; }
            set { _languageStatisticsView = value; RaisePropertyChanged("LanguageStatisticsView"); }
        }

        private LanguageStatisticsView _languageStatisticsView;
    }
}
