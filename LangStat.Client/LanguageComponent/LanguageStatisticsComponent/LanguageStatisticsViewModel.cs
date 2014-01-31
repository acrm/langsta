using Infrastructure.Client;
using LangStat.Core;
using LangStat.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangStat.Client.LanguageComponent.LanguageStatisticsComponent
{
    public class LanguageStatisticsViewModel : ViewModelBase
    {
        private readonly StatisticsProcessor _statisticsProcessor;
        private readonly Language _language;

        public LanguageStatisticsViewModel(Language language, StatisticsProcessor statisticsProcessor)
        {
            _language = language;
            _statisticsProcessor = statisticsProcessor;

            UpdateCommand = new DelegateCommand(Update);
        }

        public DelegateCommand UpdateCommand { get; private set; }

        private void Update()
        {
            var output = _statisticsProcessor.BuildLanguageStatistics(_language.Name);
            Output = output;
        }

        public string Output
        {
            get { return _output; }
            set 
            {
                _output = value;
                RaisePropertyChanged("Output");
            }
        }

        private string _output;

    }
}
