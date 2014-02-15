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
            _languageSourcesDao.LanguageSourceDeleted += OnDaoLanguageSourceDeleted;

            var languageSources = _languageSourcesDao.GetAllLanguageSources(_languageName);
            foreach (var languageSourceDto in languageSources ?? new LanguageSourceDto[0])
            {
                var languageSource = CreateLanguageSourceFromDto(languageSourceDto);
                _sourcesCache.Add(languageSource.Id, languageSource);
            }
        }

        #region Добавление

        public LanguageSourceCreationResponse CreateLanguageSource(LanguageSourceCreationRequest request)
        {
            var newLanguageSourceId = Guid.NewGuid();

            var languageSourceDto = new LanguageSourceDto
            {
                Id = newLanguageSourceId,
                Address = request.Address
            };

            var isSuccessful = _languageSourcesDao.AddLanguageSource(request.LanguageName, languageSourceDto);
            if (!isSuccessful) return new LanguageSourceCreationResponse { IsSuccessful = false };
            
            return new LanguageSourceCreationResponse 
            {
                Id = newLanguageSourceId,
                IsSuccessful = true
            };
        }

        private void OnDaoLanguageSourceAdded(object sender, LanguageSourceDto addedLanguageSource)
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
                RaiseLanguageSourceAdded(languageSource);
            }
        }
        
        public event EventHandler<LanguageSource> LanguageSourceAdded;

        private void RaiseLanguageSourceAdded(LanguageSource addedLanguageSource)
        {
            var handler = LanguageSourceAdded;
            if (handler != null)
            {
                handler.Invoke(this, addedLanguageSource);
            }
        }

        #endregion Добавление

        #region Удаление

        public bool DeleteLanguageSource(Guid languageSourceId)
        {
            return _languageSourcesDao.DeleteLanguageSource(_languageName, languageSourceId);
        }

        private void OnDaoLanguageSourceDeleted(object sender, LanguageSourceDto deletedLanguageSourceDto)
        {
            if (deletedLanguageSourceDto == null) return;

            if (!_sourcesCache.ContainsKey(deletedLanguageSourceDto.Id)) return;

            _sourcesCache.Remove(deletedLanguageSourceDto.Id);
            var deletedLanguageSource = CreateLanguageSourceFromDto(deletedLanguageSourceDto);
            RaiseLanguageSourceDeleted(deletedLanguageSource);
        }
        
        public event EventHandler<LanguageSource> LanguageSourceDeleted;
        
        private void RaiseLanguageSourceDeleted(LanguageSource deletedLanguageSource)
        {
            var handler = LanguageSourceDeleted;
            if (handler != null)
            {
                handler.Invoke(this, deletedLanguageSource);
            }
        }
        
        #endregion Удаление

        #region Получение

        public LanguageSource GetLanguageSource(Guid languageSourceId)
        {
            if (!_sourcesCache.ContainsKey(languageSourceId)) return null;

            return _sourcesCache[languageSourceId];
        }

        public LanguageSource[] GetAllLanguageSources()
        {
            return _sourcesCache.Values.ToArray();
        }

        #endregion Получение

        public event EventHandler<LanguageSource> LanguageSourceUpdated;

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

    }
}
