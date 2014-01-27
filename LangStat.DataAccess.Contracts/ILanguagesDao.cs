using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangStat.DataAccess.Contracts
{
    public interface ILanguagesDao
    {
        bool AddLanguage(LanguageDto language);

        bool UpdateLanguage(LanguageDto updatedLanguage);

        bool DeleteLanguage(LanguageDto language);

        LanguageDto GetLanguage(string languageName);

        LanguageDto[] GetAllLanguages();

        event EventHandler<LanguageDto> LanguageAdded;

        event EventHandler<LanguageDto> LanguageDeleted;
        
        event EventHandler<LanguageDto> LanguageUpdated;
    }
}
