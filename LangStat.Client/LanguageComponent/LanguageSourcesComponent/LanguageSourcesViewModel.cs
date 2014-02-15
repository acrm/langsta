using Infrastructure.Client;
using Infrastructure.Client.Dialog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LangStat.Client.LanguageComponent;
using LangStat.Core.Contracts;

namespace LangStat.Client.LanguageSourcesComponent
{
    public class LanguageSourcesViewModel : ViewModelBase
    {
        private readonly ILanguageSourcesRepository _sourcesRepository;
        private readonly Language _language;

        public LanguageSourcesViewModel(Language language, ILanguageSourcesRepository sourcesRepository)
        {
            _language = language;
            _sourcesRepository = sourcesRepository;
            _sourcesRepository.LanguageSourceAdded += OnRepositoryLanguageSourceAdded;
            _sourcesRepository.LanguageSourceDeleted += OnRepositoryLanguageSourceDeleted;

            AddCommand = new DelegateCommand(Add);
            DeleteCommand = new DelegateCommand(Delete);

            var items = _sourcesRepository.GetAllLanguageSources()
                .Select(source => new LanguageSourceViewModel(source));

            Items = new ObservableCollection<LanguageSourceViewModel>(items);
        }
        
        public ObservableCollection<LanguageSourceViewModel> Items { get; private set; }

        public LanguageSourceViewModel SelectedItem
        {
            get { return _selectedItem; }
            set { _selectedItem = value; RaisePropertyChanged("SelectedItem"); }
 
        }

        private LanguageSourceViewModel _selectedItem;

        private void Delete()
        {
            if (SelectedItem == null) return;
            
            _sourcesRepository.DeleteLanguageSource(SelectedItem.Id);
        }

        void OnRepositoryLanguageSourceDeleted(object sender, LanguageSource deletedLanguageSource)
        {
            var deletedItem = Items.FirstOrDefault(item => item.Id == deletedLanguageSource.Id);
            if (deletedItem == null) return;

            Items.Remove(deletedItem);
        }

        private void Add()
        {
            var editViewModel = new EditLanguageSourceViewModel(_language);
            var editView = new EditLanguageSourceView();

            DialogHelper.ShowDialog(new Size(300, 150), "Добавение источника языка", editView, editViewModel, () =>
                {
                    var languageSourceCreationRequest = editViewModel.GetCreationRequest();

                    _sourcesRepository.CreateLanguageSource(languageSourceCreationRequest);
                });
        }

        void OnRepositoryLanguageSourceAdded(object sender, LanguageSource addedLanguageSource)
        {
            var languageSourceViewModel = new LanguageSourceViewModel(addedLanguageSource);
            Items.Add(languageSourceViewModel);
        }

        public DelegateCommand AddCommand { get; private set; }

        public DelegateCommand DeleteCommand { get; private set; }

        public DelegateCommand RefreshCommand { get; private set; }
    }
}
