using LangStat.Core.Contracts;
using LangStat.DataAccess.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangStat.Core
{
    public class LanguageSourcesRepository : ILanguageSourcesRepository
    {
        private readonly string _languageName;
        private readonly ILanguageSourcesDao _languageSourcesDao;
        private readonly Dictionary<Guid, LanguageSource> _sourcesCache;

        public LanguageSourcesRepository(string languageName, ILanguageSourcesDao languageSourcesDao)
        {
            _languageName = languageName;
            _languageSourcesDao = languageSourcesDao;
            _sourcesCache = new Dictionary<Guid, LanguageSource>();

            _languageSourcesDao.LanguageSourceAdded += OnDaoLanguageSourceAdded;

            var languageSources = _languageSourcesDao.GetLanguageSources(_languageName);
            foreach (var languageSourceDto in languageSources ?? new LanguageSourceDto[0])
            {
                var languageSource = CreateLanguageSourceFromDto(languageSourceDto);
                _sourcesCache.Add(languageSource.Id, languageSource);
            }
        }

        void OnDaoLanguageSourceAdded(object sender, LanguageSourceDto addedLanguageSource)
        {
            if (addedLanguageSource == null) return;

            var languageSourceKey = addedLanguageSource.Id;
            var languageSource = CreateLanguageSourceFromDto(addedLanguageSource);

            if (_sourcesCache.ContainsKey(languageSourceKey))
            {
                _sourcesCache[languageSourceKey] = languageSource;
            }
            else 
            {
                _sourcesCache.Add(languageSourceKey, languageSource);
            }
        }

        private LanguageSource CreateLanguageSourceFromDto(LanguageSourceDto languageSourceDto)
        {
            if (languageSourceDto == null) return null;

            var languageSource = new LanguageSource(_languageName)
            {
                Id = languageSourceDto.Id,
                Address = languageSourceDto.Address
            };

            return languageSource;
        }

        public LanguageSourceCreationResponse CreateLanguageSource(LanguageSourceCreationRequest request)
        {
            var languageSource = new LanguageSource(_languageName)
            {
                Id = Guid.NewGuid(),
                Address = request.Address
            };

            var languageSourceDto = new LanguageSourceDto
            {
                Id = languageSource.Id,
                Address = languageSource.Address
            };

            var isSuccessful = _languageSourcesDao.AddLanguageSource(request.LanguageName, languageSourceDto);
            if (!isSuccessful) return new LanguageSourceCreationResponse { IsSuccessful = false };
            
            return new LanguageSourceCreationResponse 
            {
                Id = languageSource.Id,
                IsSuccessful = true
            };
        }

        public LanguageSource GetLanguageSource(Guid languageSourceId)
        {
            if (!_sourcesCache.ContainsKey(languageSourceId)) return null;

            return _sourcesCache[languageSourceId];
        }

        public LanguageSource[] GetAllLanguageSources()
        {
            return _sourcesCache.Values.ToArray();
        }
    }
}
