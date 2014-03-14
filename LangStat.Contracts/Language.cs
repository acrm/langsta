using LangStat.Core.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LangStat.Core.Contracts
{
    public class Language
    {
        private readonly ILanguageSourcesRepository _sourcesRepository;
        private readonly IIgnoredWordsRepository _ignoredWordsRepository;

        public Language(
            string languageTitle,
            ILanguageSourcesRepository sourcesRepository,
            IIgnoredWordsRepository ignoredWordsRepository)
        {
            Name = languageTitle;
            _sourcesRepository = sourcesRepository;
            _ignoredWordsRepository = ignoredWordsRepository;
        }

        public string Name { get; private set; }

        public ILanguageSourcesRepository LanguageSourcesRepository
        {
            get { return _sourcesRepository; }
        }
        
        public IIgnoredWordsRepository IgnoredWordsRepository
        {
            get { return _ignoredWordsRepository; }
        }
        
    }
}
