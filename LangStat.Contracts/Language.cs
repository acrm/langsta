using LangStat.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LangStat.Contracts
{
    public class Language
    {
        private readonly ILanguageSourcesRepository _sourcesRepository;

        public Language(string languageTitle, ILanguageSourcesRepository sourcesRepository)
        {
            Name = languageTitle;
            _sourcesRepository = sourcesRepository;
        }
        
        public ILanguageSourcesRepository LanguageSourcesRepository
        {
            get { return _sourcesRepository; }
        }

        public string Name { get; private set; }

        
    }
}
