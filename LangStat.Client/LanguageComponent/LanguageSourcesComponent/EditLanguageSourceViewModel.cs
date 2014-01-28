using Infrastructure.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LangStat.Client.LanguageComponent;
using LangStat.Core.Contracts;

namespace LangStat.Client.LanguageSourcesComponent
{
    public class EditLanguageSourceViewModel : ViewModelBase
    {
        private readonly LanguageSourceCreationRequest _creationRequest;

        public EditLanguageSourceViewModel(Language language, LanguageSource languageSourceToEdit = null)
        {
            _creationRequest = new LanguageSourceCreationRequest();
            if (languageSourceToEdit != null)
            {
                _creationRequest.Address = languageSourceToEdit.Address;
            }

            _creationRequest.LanguageName = language.Name;
        }

        public string Address
        {
            get { return _address; }
            set
            {
                _address = value;
                _creationRequest.Address = _address;
                Validate();
                RaisePropertyChanged("Address");
            }
        }

        private string _address;

        public LanguageSourceCreationRequest GetCreationRequest()
        {
            return _creationRequest;
        }

        public override bool IsValid
        {
            get
            {
                var isValid = !string.IsNullOrWhiteSpace(_creationRequest.Address);
                return isValid && base.IsValid;
            }
        }
    }
}
