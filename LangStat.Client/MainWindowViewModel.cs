using Infrastructure.Client;
using Infrastructure.Client.Dialog;
using LangStat.Core.Contracts;
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
using System.Collections.Specialized;

namespace LangStat.Client
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly MainWindow _mainWindow;
        private readonly TabsViewModel _tabsViewModel;
        
        public MainWindowViewModel(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;

            _mainWindow.LanguagesRepository.LanguageAdded += OnRepositoryLanguageAdded;
            _mainWindow.LanguagesRepository.LanguageDeleted += OnRepositoryLanguageDeleted;

            _tabsViewModel = new TabsViewModel();
            LanguagesTabsView = new TabsView { Model = _tabsViewModel };
                        
            AddLanguageCommand = new DelegateCommand(AddLanguage);
            RemoveLanguageCommand = new DelegateCommand(RemoveCurrentLanguage, CanRemoveCurrentLanguage);
            
            Languages = new ObservableCollection<LanguageViewModel>();
            Languages.CollectionChanged += Languages_CollectionChanged;

            var languages = _mainWindow.LanguagesRepository.GetAllLanguages();
            foreach (var language in languages ?? new Language[0])
            {
                var languageViewModel = new LanguageViewModel(language, mainWindow.StatisticsProcessor);
                Languages.Add(languageViewModel);
                _tabsViewModel.OpenTab(language, mainWindow.StatisticsProcessor);
            }
        }

        private void OnRepositoryLanguageDeleted(object sender, Language deletedLanguage)
        {
            if (deletedLanguage == null || deletedLanguage.Name == null) return;

            var deletedLanguageiewModel = Languages
                .FirstOrDefault(languageViewModel => languageViewModel.Language.Name == deletedLanguage.Name);
            if (deletedLanguageiewModel == null) return;

            Languages.Remove(deletedLanguageiewModel);
            _tabsViewModel.CloseTab(deletedLanguage.Name);

            RefreshCommands();
        }

        private void OnRepositoryLanguageAdded(object sender, Language addedLanguage)
        {
            if (addedLanguage == null || addedLanguage.Name == null) return;

            var languageViewModel = new LanguageViewModel(addedLanguage, _mainWindow.StatisticsProcessor);
            Languages.Add(languageViewModel);
            _tabsViewModel.OpenTab(addedLanguage, _mainWindow.StatisticsProcessor);
            
            RefreshCommands();
        }

        private bool CanRemoveCurrentLanguage()
        {
            var selectedLanguageTab = _tabsViewModel.SelectedItem;
            if (selectedLanguageTab == null) return false;

            return true;
        }

        private void RemoveCurrentLanguage()
        {
            var selectedLanguageTab = _tabsViewModel.SelectedItem;
            if (selectedLanguageTab == null) return;

            var currentLanguageComponent = selectedLanguageTab.LanguageComponent;
            if (currentLanguageComponent == null) return;

            var current = currentLanguageComponent.Model.Language;
            _mainWindow.LanguagesRepository.DeleteLanguage(current);
        }

        void Languages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                HasLanguages = true;
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove 
                     && (e.NewItems == null || e.NewItems.Count == 0))
            {
                HasLanguages = false;
            }
        }

        public ObservableCollection<LanguageViewModel> Languages { get; private set; }

        public DelegateCommand AddLanguageCommand { get; private set; }

        public DelegateCommand RemoveLanguageCommand { get; private set; }

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

                _tabsViewModel.OpenTab(addedLanguage, _mainWindow.StatisticsProcessor);
                RefreshCommands();
            });

        }

        public bool HasLanguages
        {
            get { return _hasLanguages; }
            set 
            {
                _hasLanguages = value;
                RaisePropertyChanged("HasLanguages");
            }
        }

        private bool _hasLanguages;

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

        private void RefreshCommands()
        {
            RemoveLanguageCommand.RaiseCanExecuteChanged();
        }
    }
}
