using Infrastructure.Client;
using LangStat.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangStat.Client.LanguageSourcesComponent
{
    public class LanguageSourceViewModel :  ViewModelBase
    {
        private readonly LanguageSource _languageSource;

        public LanguageSourceViewModel(LanguageSource languageSource)
        {
            _languageSource = languageSource;
            Address = _languageSource.Address;
        }

        public Guid Id
        {
            get { return _languageSource.Id; }
        }

        public string Address
        {
            get { return _address; }
            set
            {
                _address = value;
                RaisePropertyChanged("Address");
            }
        }

        private string _address;

    }
}
