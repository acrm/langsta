using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangStat.Core.Contracts
{
    public interface ILanguagesRepository
    {
        LanguageCreationResponse CreateLanguage(LanguageCreationRequest request);

        bool DeleteLanguage(Language language);

        Language[] GetAllLanguages();

        Language GetLanguage(string languageName);
        
        event EventHandler<Language> LanguageAdded;

        event EventHandler<Language> LanguageDeleted;
        
        event EventHandler<Language> LanguageUpdated;
    }
}
