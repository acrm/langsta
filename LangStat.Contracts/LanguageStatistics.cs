
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangStat.Contracts
{
    public class LanguageStatistics
    {
        public int WordsTotalCount { get; set; }

        public WordStatistics[] UniqueWords { get; set; }
    }
}
