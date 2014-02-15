using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangStat.DataAccess.Contracts
{
    public interface ILanguageSourcesDao
    {
        bool AddLanguageSource(string languageName, LanguageSourceDto languageSource);

        bool DeleteLanguageSource(string languageName, Guid languageSourceId);
        
        LanguageSourceDto[] GetAllLanguageSources(string languageName);
        
        event EventHandler<LanguageSourceDto> LanguageSourceAdded;

        event EventHandler<LanguageSourceDto> LanguageSourceDeleted;
    }
}
