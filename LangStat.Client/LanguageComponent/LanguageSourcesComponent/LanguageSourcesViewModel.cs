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
using LangStat.Contracts;

namespace LangStat.Client.LanguageSourcesComponent
{
    public class LanguageSourcesViewModel
    {
        private readonly ILanguageSourcesRepository _sourcesRepository;

        public LanguageSourcesViewModel(ILanguageSourcesRepository sourcesRepository)
        {
            _sourcesRepository = sourcesRepository;

            AddCommand = new DelegateCommand(Add);
            DeleteCommand = new DelegateCommand(Delete);

            var items = _sourcesRepository.GetAllLanguageSources()
                .Select(source => new LanguageSourceViewModel(source));

            Items = new ObservableCollection<LanguageSourceViewModel>(items);
        }

        //void OnLanguageSourceAdded(object sender, LanguageSourceDto addedLanguageSource)
        //{
        //    Items.Add(new LanguageSourceViewModel(addedLanguageSource));
        //}

        public ObservableCollection<LanguageSourceViewModel> Items { get; private set; }

        private void Delete()
        {
            throw new NotImplementedException();
        }

        private void Add()
        {
            var emptyLanguageSource = new LanguageSourceDto();
            var editViewModel = new EditLanguageSourceViewModel(emptyLanguageSource);
            var editView = new EditLanguageSourceView();

            DialogHelper.ShowDialog(new Size(300, 150), "Добавение источника языка", editView, editViewModel, () =>
                {
                    var filledLanguageSource = editViewModel.GetLanguageSource();
                    var request = new LanguageSourceCreationRequest 
                    {
                        Address = filledLanguageSource.Address
                    };

                    var response = _sourcesRepository.CreateLanguageSource(request);
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
