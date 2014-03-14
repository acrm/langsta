using Infrastructure.Client;
using LangStat.Contracts;
using LangStat.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangStat.Client.LanguageComponent.LanguageStatisticsComponent
{
    public class WordViewModel : ViewModelBase
    {
        private readonly WordStatistics _wordStatistics;

        public WordViewModel(WordStatistics wordStatistics)
        {
            _wordStatistics = wordStatistics;

            Spelling = wordStatistics.Spelling;
            CountOfAccurances = wordStatistics.CountOfAccurances.ToString();
        }

        public string Spelling
        {
            get { return _spelling; }
            set { _spelling = value; RaisePropertyChanged("Spelling"); }
        }

        private string _spelling;

        public string CountOfAccurances
        {
            get { return _countOfAccurances; }
            set { _countOfAccurances = value; RaisePropertyChanged("CountOfAccurances"); }
        }

        private string _countOfAccurances;
    }
}
