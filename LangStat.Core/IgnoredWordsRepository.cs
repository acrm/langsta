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
    public class IgnoredWordsRepository : IIgnoredWordsRepository
    {
        private readonly ILanguagesDao _languagesDao;
        private readonly string _languageName;
        private readonly List<string> _wordsCache;

        public IgnoredWordsRepository(string languageName, ILanguagesDao languageDao, string[] initialIgnoredWords = null)
        {
            _languageName = languageName;
            _languagesDao = languageDao;
            _wordsCache = initialIgnoredWords != null
                ? new List<string>(initialIgnoredWords)
                : new List<string>();
        }

        public string[] GetAllWords()
        {
            return _wordsCache.ToArray();
        }

        public bool AddWord(string word)
        {
            var languageDto = _languagesDao.GetLanguage(_languageName);
            if (languageDto == null) return false;

            if (_wordsCache.Count > 0)
            {
                _wordsCache.Clear();
            }
            if (languageDto.IgnoredWords != null)
            {
                _wordsCache.AddRange(languageDto.IgnoredWords);
            }
            _wordsCache.Add(word);

            var newLanguageDto = new LanguageDto
            {
                Name = languageDto.Name,
                IgnoredWords = _wordsCache.ToArray()
            };

            var updateIsSuccessful = _languagesDao.UpdateLanguage(newLanguageDto);
            return updateIsSuccessful;
        }
    }
}
