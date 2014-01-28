using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangStat.Core.Contracts
{
    public class LanguageSourceCreationRequest
    {
        public string LanguageName { get; set; }

        /// <summary>
        /// Адрес.
        /// </summary>
        public string Address { get; set; }
    }
}
