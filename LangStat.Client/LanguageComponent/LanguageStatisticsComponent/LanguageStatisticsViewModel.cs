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
        private StatisticsProcessor _statisticsProcessor;

        public LanguageStatisticsViewModel(Language language, StatisticsProcessor statisticsProcessor)
        {
            _statisticsProcessor = statisticsProcessor;
        }

    }
}
