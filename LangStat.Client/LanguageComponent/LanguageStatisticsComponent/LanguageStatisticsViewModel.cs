using Infrastructure.Client;
using LangStat.Contracts;
using LangStat.Core;
using LangStat.Core.Contracts;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangStat.Client.LanguageComponent.LanguageStatisticsComponent
{
    public class LanguageStatisticsViewModel : ViewModelBase
    {
        private readonly StatisticsProcessor _statisticsProcessor;
        private readonly Language _language;

        private LanguageStatistics _languageStatistics;

        public LanguageStatisticsViewModel(Language language, StatisticsProcessor statisticsProcessor)
        {
            _language = language;
            _statisticsProcessor = statisticsProcessor;
            Words = new ObservableCollection<WordViewModel>();

            UpdateCommand = new DelegateCommand(Update);
            ExportCommand = new DelegateCommand(Export, CanExport);
            ExcludeCommand = new DelegateCommand(Exclude, CanExclude);
            GroupCommand = new DelegateCommand(Group, CanGroup);
        }

        #region Команды

        public DelegateCommand UpdateCommand { get; private set; }

        private async void Update()
        {
            IsBusy = true;
            var buildingResult = await _statisticsProcessor.BuildLanguageStatisticsAsync(_language.Name);
            
            if (buildingResult.ErrorMessage != null)
            {
                Output = buildingResult.ErrorMessage;
            }

            if (!buildingResult.BuildIsSuccessful) return;

            var statistics = buildingResult.Result;
            _languageStatistics = statistics;
            ExportCommand.RaiseCanExecuteChanged();

            var orderedStatistics = statistics.UniqueWords
                .OrderByDescending(wordStatistis => wordStatistis.CountOfAccurances);

            Words.Clear();
            foreach (var wordStatistics in orderedStatistics)
            {
                var word = new WordViewModel(wordStatistics);
                Words.Add(word);
            }

            IsBusy = false;
        }

        public DelegateCommand ExportCommand { get; private set; }
        
        private bool CanExport()
        {
            return _languageStatistics != null;
        }

        private void Export()
        {
            if (_languageStatistics == null) 
            {
                Output = "Экспорт отменён: отсутствует статистика.";
                return;
            }

            var text = ConvertStatisticsToText(_languageStatistics);
            if(string.IsNullOrWhiteSpace(text))
            {
                Output = "Экспорт отменён: ошибка преобразования в текстовый формат.";
                return;
            }

            var dialog = new SaveFileDialog 
                {
                    Filter = "Text files |*.txt| All files |*.*",
                    FileName = string.Format("Статистика слов. {0} язык", _language.Name)
                };
            var fileIsChosen = dialog.ShowDialog();
            if (fileIsChosen != true)
            {
                Output = "Экспорт отменён: файл не выбран.";
                return;
            }

            using (var stream = new StreamWriter(dialog.FileName))
            {
                stream.Write(text);
            }
            
            Output = string.Format("Экспорт завершён успешно: {0}", dialog.FileName);          
        }

        private string ConvertStatisticsToText(LanguageStatistics statistics)
        {
            if (statistics == null) return null;

            var builder = new StringBuilder();

            builder.AppendFormat("Всего слов: {0}\r\n", statistics.WordsTotalCount);

            var orderedWords = statistics.UniqueWords
                .OrderByDescending(word => word.CountOfAccurances);

            foreach (var word in orderedWords)
            {
                builder.AppendFormat("{0}: {1}\r\n", word.Spelling, word.CountOfAccurances);
            }
            
            return builder.ToString();
        }
                
        public DelegateCommand ExcludeCommand { get; private set; }

        private bool CanExclude()
        {
            return SelectedWord != null;
        }

        private void Exclude()
        {
            if (SelectedWord == null) return;

            var wordIgnored = _language.IgnoredWordsRepository.AddWord(SelectedWord.Spelling);
            if (!wordIgnored) return;

            Words.Remove(SelectedWord);
        }

        public DelegateCommand GroupCommand { get; private set; }

        private bool CanGroup()
        {
            throw new NotImplementedException();
        }

        private void Group()
        {
            throw new NotImplementedException();
        }

        #endregion Команды

        public ObservableCollection<WordViewModel> Words { get; private set; }

        public WordViewModel SelectedWord
        {
            get { return _selectedWord; }
            set 
            {
                _selectedWord = value;
                RaisePropertyChanged("SelectedWord");
                ExcludeCommand.RaiseCanExecuteChanged();
            }
        }

        private WordViewModel _selectedWord;
                        
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
