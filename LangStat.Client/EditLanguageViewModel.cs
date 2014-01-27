using Infrastructure.Client;
using LangStat.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LangStat.Client.LanguageComponent;

namespace LangStat.Client
{
    public class EditLanguageViewModel : ViewModelBase
    {
        private readonly LanguageCreationRequest _creationRequest;

        public EditLanguageViewModel(Language languageToEdit = null)
        {
            _creationRequest = new LanguageCreationRequest();

            if (languageToEdit != null) 
            {
                _creationRequest.Name = languageToEdit.Name;
            }
        }

        public bool TitleIsValid
        {
            get 
            {
                return !string.IsNullOrWhiteSpace(Title);
            }
        }

        public string Title
        {
            get { return _title; }
            set 
            {
                _title = value;
                if (TitleIsValid)
                {
                    _creationRequest.Name = _title;
                }

                Validate();
                RaisePropertyChanged("Title");
            }
        }

        private string _title;

        public override bool IsValid
        {
            get
            {
                return base.IsValid
                    && TitleIsValid;
            }
        }

        public LanguageCreationRequest  GetCreationRequest()
        {
            return _creationRequest;
        }
    }
}
