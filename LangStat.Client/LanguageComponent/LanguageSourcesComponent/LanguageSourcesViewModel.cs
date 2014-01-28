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
    public class LanguageSourcesViewModel
    {
        private readonly ILanguageSourcesRepository _sourcesRepository;
        private readonly Language _language;

        public LanguageSourcesViewModel(Language language, ILanguageSourcesRepository sourcesRepository)
        {
            _language = language;
            _sourcesRepository = sourcesRepository;

            AddCommand = new DelegateCommand(Add);
            DeleteCommand = new DelegateCommand(Delete);

            var items = _sourcesRepository.GetAllLanguageSources()
                .Select(source => new LanguageSourceViewModel(source));

            Items = new ObservableCollection<LanguageSourceViewModel>(items);
        }
        
        public ObservableCollection<LanguageSourceViewModel> Items { get; private set; }

        private void Delete()
        {
            throw new NotImplementedException();
        }

        private void Add()
        {
            var editViewModel = new EditLanguageSourceViewModel(_language);
            var editView = new EditLanguageSourceView();

            DialogHelper.ShowDialog(new Size(300, 150), "Добавение источника языка", editView, editViewModel, () =>
                {
                    var languageSourceCreationRequest = editViewModel.GetCreationRequest();

                    var response = _sourcesRepository.CreateLanguageSource(languageSourceCreationRequest);
                    if (response == null || !response.IsSuccessful) return;

                    var addedLanguageSource = _sourcesRepository.GetLanguageSource(response.Id);
                    var languageSourceViewModel = new LanguageSourceViewModel(addedLanguageSource);
                    Items.Add(languageSourceViewModel);
                });
        }

        public DelegateCommand AddCommand { get; private set; }

        public DelegateCommand DeleteCommand { get; private set; }

        public DelegateCommand RefreshCommand { get; private set; }
    }
}
