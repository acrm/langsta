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

        LanguageSource GetLanguageSource(Guid languageSourceId);
        
        LanguageSource[] GetAllLanguageSources();


    }
}
