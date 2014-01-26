using Infrastructure.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LangStat.Client.LanguageComponent;
using LangStat.Contracts;

namespace LangStat.Client.LanguageSourcesComponent
{
    public class EditLanguageSourceViewModel : ViewModelBase
    {
        private readonly LanguageSourceDto _languageSource;

        public EditLanguageSourceViewModel(LanguageSourceDto languageSourceToEdit)
        {
            _languageSource = languageSourceToEdit;
        }

        public string Address
        {
            get { return _address; }
            set
            {
                _address = value;
                _languageSource.Address = _address;
                Validate();
                RaisePropertyChanged("Address");
            }
        }

        private string _address;

        public LanguageSourceDto GetLanguageSource()
        {
            return _languageSource;
        }

        public override bool IsValid
        {
            get
            {
                var isValid = !string.IsNullOrWhiteSpace(_languageSource.Address);
                return isValid && base.IsValid;
            }
        }
    }
}
