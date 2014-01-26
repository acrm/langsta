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
        private readonly WordsRepository _wordsRepository;
        private readonly ILanguagesDao _languagesDao;
        private readonly ILanguageSourcesDao _languageSourcesDao;
        private readonly Dictionary<string, Language> _languages = new Dictionary<string, Language>();

        public LanguagesRepository(ILanguagesDao languagesDao)
        {
            _languagesDao = languagesDao;
            _wordsRepository = new WordsRepository();

            var languageDtos = _languagesDao.GetAllLanguages();
                                    
            foreach (var languageDto in languageDtos ?? new LanguageEntity[0])
            {
                var language = CreateLanguageFromEntity(languageDto);
                if (language == null) continue;
                _languages.Add(language.Name, language);
            }
        }

        private static Language CreateLanguageFromDto(LanguageDto languageDto)
        {
            if (languageDto == null) return null;

            var dao = new LanguageSourcesDao(languageDto.Name);
            var repository = new LanguageSourcesRepository(null, dao);
            return new Language(languageDto.Name, repository);
        }

        private static Language CreateLanguageFromEntity(LanguageEntity languageEntity)
        {
            if (languageEntity == null) return null;
            var dao = new LanguageSourcesDao(languageEntity.Name);
            var repo = new LanguageSourcesRepository(null, dao);
            return new Language(languageEntity.Name, repo);
        }

        public Language GetLanguage(string languageName)
        {
            if (string.IsNullOrWhiteSpace(languageName)) return null;
            if (!_languages.ContainsKey(languageName)) return null;

            var language = _languages[languageName];
            if (language == null) return null;

            return language;
        }

        public LanguageCreationResponse CreateLanguage(LanguageCreationRequest request)
        {
            if (request == null) return new LanguageCreationResponse { IsSuccessful = false };
            if (string.IsNullOrWhiteSpace(request.Name)) return new LanguageCreationResponse { IsSuccessful = false };
            if (_languages.ContainsKey(request.Name)) return new LanguageCreationResponse { IsSuccessful = false };


            var dao = new LanguageSourcesDao(request.Name);
            var repo = new LanguageSourcesRepository(null, dao);
            var language = new Language(request.Name, repo);
            var languageEntity = new LanguageEntity { Name = language.Name };
            var isSuccesful = _languagesDao.AddLanguage(languageEntity);
            if (!isSuccesful) return new LanguageCreationResponse { IsSuccessful = false };
            
            _languages.Add(request.Name, language);

            return new LanguageCreationResponse 
            {
                IsSuccessful = true,
                Name = request.Name
            };
        }

        public Language[] GetAllLanguages()
        {
            return _languages.Values
                .Where(language => language != null)
                .ToArray();
        }
    }
}
