using LangStat.Contracts;
using LangStat.DataAccess;
using LangStat.DataAccess.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangStat.Core
{
    public class LanguagesRepository : ILanguagesRepository
    {
        private readonly ILanguagesDao _languagesDao;
        private readonly ILanguageSourcesDao _languageSourcesDao;
        private readonly Dictionary<string, Language> _languagesCache;

        public LanguagesRepository(ILanguagesDao languagesDao, ILanguageSourcesDao languageSourcesDao)
        {
            _languagesDao = languagesDao;
            _languageSourcesDao = languageSourcesDao;
            _languagesCache = new Dictionary<string, Language>();

            _languagesDao.LanguageAdded += OnDaoLanguageAdded;

            var languageDtos = _languagesDao.GetAllLanguages();
                                    
            foreach (var languageDto in languageDtos ?? new LanguageDto[0])
            {
                var language = CreateLanguageFromDto(languageDto);
                if (language == null) continue;
                _languagesCache.Add(language.Name, language);
            }
        }

        void OnDaoLanguageAdded(object sender, LanguageDto addedLanguage)
        {
            if (addedLanguage == null || addedLanguage.Name == null) return;

            var languageKey = addedLanguage.Name;
            var language = CreateLanguageFromDto(addedLanguage);

            if (_languagesCache.ContainsKey(languageKey))
            {
                _languagesCache[languageKey] = language;
            }
            else 
            {
                _languagesCache.Add(languageKey, language);
            }
        }

        private Language CreateLanguageFromDto(LanguageDto languageDto)
        {
            if (languageDto == null) return null;
            var languageSourcesRepository = new LanguageSourcesRepository(languageDto.Name, _languageSourcesDao);
            return new Language(languageDto.Name, languageSourcesRepository);
        }

        public Language GetLanguage(string languageName)
        {
            if (string.IsNullOrWhiteSpace(languageName)) return null;
            if (!_languagesCache.ContainsKey(languageName)) return null;

            var language = _languagesCache[languageName];
            if (language == null) return null;

            return language;
        }

        public LanguageCreationResponse CreateLanguage(LanguageCreationRequest request)
        {
            if (request == null) return new LanguageCreationResponse { IsSuccessful = false };
            if (string.IsNullOrWhiteSpace(request.Name)) return new LanguageCreationResponse { IsSuccessful = false };
            if (_languagesCache.ContainsKey(request.Name)) return new LanguageCreationResponse { IsSuccessful = false };


            var languageSourcesRepository = new LanguageSourcesRepository(request.Name, _languageSourcesDao);
            var language = new Language(request.Name, languageSourcesRepository); 
            var languageDto = new LanguageDto { Name = language.Name };
            var isSuccesful = _languagesDao.AddLanguage(languageDto);
            if (!isSuccesful) return new LanguageCreationResponse { IsSuccessful = false };
            
            return new LanguageCreationResponse 
            {
                IsSuccessful = true,
                Name = request.Name
            };
        }

        public Language[] GetAllLanguages()
        {
            return _languagesCache.Values
                .Where(language => language != null)
                .ToArray();
        }
    }
}
