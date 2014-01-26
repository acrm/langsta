using LangStat.Contracts;
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
        private readonly ILanguageSourcesDao _languageSourcesDao;
        private readonly Dictionary<Guid, LanguageSource> _sourcesCache;

        public LanguageSourcesRepository(Language language, ILanguageSourcesDao languageSourcesDao)
        {
            Language = language;
            _languageSourcesDao = languageSourcesDao;
            _sourcesCache = new Dictionary<Guid, LanguageSource>();

            var languageSources = _languageSourcesDao.GetAllLanguageSources();
            foreach (var languageSourceEntity in languageSources ?? new LanguageSourceEntity[0])
            {
                var languageSource = new LanguageSource(Language) 
                {
                    Id = languageSourceEntity.Id,
                    Address = languageSourceEntity.Address
                };

                _sourcesCache.Add(languageSource.Id, languageSource);
            }
        }

        public Language Language { get; private set; }

        public LanguageSourceCreationResponse CreateLanguageSource(LanguageSourceCreationRequest request)
        {
            var languageSource = new LanguageSource(Language)
            {
                Id = Guid.NewGuid(),
                Address = request.Address
            };

            var languageSourceEntity = new LanguageSourceEntity
            {
                Id = languageSource.Id,
                Address = languageSource.Address
            };

            var isSuccessful = _languageSourcesDao.AddLanguageSource(languageSourceEntity);
            if (!isSuccessful) return new LanguageSourceCreationResponse { IsSuccessful = false };
            
            _sourcesCache.Add(languageSource.Id, languageSource);

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
