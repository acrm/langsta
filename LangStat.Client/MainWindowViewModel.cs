using Infrastructure.Client;
using Infrastructure.Client.Dialog;
using LangStat.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LangStat.Client.TabsComponent;
using Xceed.Wpf.Toolkit;
using System.Collections.ObjectModel;
using LangStat.Core;
using LangStat.Client.LanguageComponent;

namespace LangStat.Client
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly MainWindow _mainWindow;
        private readonly TabsViewModel _tabsViewModel;
        
        public MainWindowViewModel(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;

            _tabsViewModel = new TabsViewModel();
            LanguagesTabsView = new TabsView { Model = _tabsViewModel };
                        
            AddLanguageCommand = new DelegateCommand(AddLanguage);

            Languages = new ObservableCollection<LanguageViewModel>();

            var languages = _mainWindow.LanguagesRepository.GetAllLanguages();
            foreach (var language in languages ?? new Language[0])
            {
                var languageViewModel = new LanguageViewModel(language);
                Languages.Add(languageViewModel);
                _tabsViewModel.OpenTab(language);
            }
        }

        public ObservableCollection<LanguageViewModel> Languages { get; private set; }
        
        public DelegateCommand AddLanguageCommand { get; private set; }

        private void AddLanguage()
        {
            var editViewModel = new EditLanguageViewModel();
            var editView = new EditLanguageView();
            DialogHelper.ShowDialog(new Size(300, 120), "Добавление языка", editView, editViewModel, () =>
            {
                var languageCreationRequest = editViewModel.GetCreationRequest();

                var languagesRepository = _mainWindow.LanguagesRepository;
                if (languagesRepository == null) return;

                var response = languagesRepository.CreateLanguage(languageCreationRequest);
                if (response == null || !response.IsSuccessful) return;

                var addedLanguage = languagesRepository.GetLanguage(response.Name);

                _tabsViewModel.OpenTab(addedLanguage);
            });

        }

        public TabsView LanguagesTabsView
        {
            get { return _languagesTabsView; }
            set 
            {
                _languagesTabsView = value; 
                RaisePropertyChanged("LanguagesTabsView"); 
            }
        }

        private TabsView _languagesTabsView;
    }
}
