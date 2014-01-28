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

        Language[] GetAllLanguages();

        Language GetLanguage(string languageName);
    }
}
