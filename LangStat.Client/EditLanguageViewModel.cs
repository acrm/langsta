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
        private readonly LanguageDto _language;

        public EditLanguageViewModel(LanguageDto language)
        {
            _language = language; 
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
                    _language.Name = _title;
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

        public LanguageDto GetLanguage()
        {
            return _language;
        }
    }
}
