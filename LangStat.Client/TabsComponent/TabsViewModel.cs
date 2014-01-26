using Infrastructure.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LangStat.Client.LanguageComponent;
using LangStat.Client.LanguagesTabsComponent;
using LangStat.Contracts;

namespace LangStat.Client.TabsComponent
{
    public class TabsViewModel : ViewModelBase
    {
        private readonly Dictionary<string, Language> _languagesCache;
        
        public TabsViewModel()
        {
            _languagesCache = new Dictionary<string, Language>();
            Items = new ObservableCollection<LanguagesTabsItemViewModel>();
        }

        public void OpenTab(Language language)
        {
            if (language == null) return;

            var languageTitle = language.Name;
            if (languageTitle == null) return;

            LanguagesTabsItemViewModel currentItem;
            if (_languagesCache.ContainsKey(languageTitle))
            {
                var currentLanguage = _languagesCache[languageTitle];

                currentItem = Items.FirstOrDefault(languageVM =>
                    languageVM.LanguageTitle == currentLanguage.Name);
            }
            else
            {
                _languagesCache.Add(languageTitle, language);
                
                currentItem = new LanguagesTabsItemViewModel(language);
                Items.Add(currentItem);
            }
            
            if(currentItem == null) return;

            SelectedItem = currentItem;
        }

        public void CloseTab(string languageName)
        {
            if (string.IsNullOrWhiteSpace(languageName)) return;
            if (!_languagesCache.ContainsKey(languageName)) return;

            var removedLanguage = _languagesCache[languageName];
            if (removedLanguage == null) return;

            var removedItem = Items
                .FirstOrDefault(languageVM => languageVM.LanguageTitle == removedLanguage.Name);
            if (removedItem == null) return;

            Items.Remove(removedItem);
            _languagesCache.Remove(languageName);
        }
        
        public ObservableCollection<LanguagesTabsItemViewModel> Items { get; private set; }

        public LanguagesTabsItemViewModel SelectedItem 
        {
            get { return _selectedItem; }
            set 
            {
                _selectedItem = value;
                RaisePropertyChanged("SelectedItem");
            }            
        }

        private LanguagesTabsItemViewModel _selectedItem;
    }
}
