using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangStat.DataAccess.Contracts
{
    public class LanguageDto
    {
        /// <summary>
        /// Название языка.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Исключённые слова.
        /// </summary>
        public string[] IgnoredWords { get; set; } 
    }
}
