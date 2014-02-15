using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangStat.Core.Contracts
{
    public interface ILanguageSourcesRepository
    {
        LanguageSourceCreationResponse CreateLanguageSource(LanguageSourceCreationRequest request);

        bool DeleteLanguageSource(Guid languageSourceId);

        LanguageSource GetLanguageSource(Guid languageSourceId);
        
        LanguageSource[] GetAllLanguageSources();

        event EventHandler<LanguageSource> LanguageSourceAdded;

        event EventHandler<LanguageSource> LanguageSourceDeleted;

        event EventHandler<LanguageSource> LanguageSourceUpdated;
    }
}
