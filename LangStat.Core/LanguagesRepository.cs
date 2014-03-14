using LangStat.Core.Contracts;
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
            _languagesDao.LanguageDeleted += OnDaoLanguageDeleted;
            _languagesDao.LanguageUpdated += OnDaoLanguageUpdated;

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

                RaiseLanguageAdded(language);
            }
        }

        void OnDaoLanguageDeleted(object sender, LanguageDto removedLanguage)
        {
            if (removedLanguage == null || removedLanguage.Name == null) return;

            var languageKey = removedLanguage.Name;
            if (!_languagesCache.ContainsKey(languageKey)) return;

            var successful = _languagesCache.Remove(languageKey);
            if (!successful) return;

            var language = CreateLanguageFromDto(removedLanguage);
            RaiseLanguageDeleted(language);
        }

        void OnDaoLanguageUpdated(object sender, LanguageDto updatedLanguage)
        {
            if (updatedLanguage == null || updatedLanguage.Name == null) return;

            var languageKey = updatedLanguage.Name;
            if (!_languagesCache.ContainsKey(languageKey)) return;

            var language = CreateLanguageFromDto(updatedLanguage);
            _languagesCache[languageKey] = language;

            RaiseLanguageUpdated(language);
        }

        private Language CreateLanguageFromDto(LanguageDto languageDto)
        {
            if (languageDto == null) return null;
            var languageSourcesRepository = new LanguageSourcesRepository(languageDto.Name, _languageSourcesDao);
            var ignoredWordsRepository = new IgnoredWordsRepository(languageDto.Name, _languagesDao, languageDto.IgnoredWords);
            return new Language(languageDto.Name, languageSourcesRepository, ignoredWordsRepository);
        }

        private LanguageDto CreateLanguageDto(Language language)
        {
            if (language == null) return null;

            return new LanguageDto 
            {
                Name = language.Name
            };
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
            var ignoredWordsRepository = new IgnoredWordsRepository(request.Name, _languagesDao);
            var language = new Language(request.Name, languageSourcesRepository, ignoredWordsRepository); 
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
        
        public bool DeleteLanguage(Language language)
        {
            if (language == null) return false;

            var languageDto = CreateLanguageDto(language);

            return _languagesDao.DeleteLanguage(languageDto);
        }

        private void RaiseLanguageAdded(Language addedLanguage)
        {
            var handler = LanguageAdded;
            if (handler != null)
            {
                handler.Invoke(this, addedLanguage);
            }
        }

        public event EventHandler<Language> LanguageAdded;

        private void RaiseLanguageDeleted(Language deletedLanguage)
        {
            var handler = LanguageDeleted;
            if (handler != null)
            {
                handler.Invoke(this, deletedLanguage);
            }
        }

        public event EventHandler<Language> LanguageDeleted;

        private void RaiseLanguageUpdated(Language updatedLanguage)
        {
            var handler = LanguageUpdated;
            if (handler != null)
            {
                handler.Invoke(this, updatedLanguage);
            }
        }

        public event EventHandler<Language> LanguageUpdated;
    }
}
