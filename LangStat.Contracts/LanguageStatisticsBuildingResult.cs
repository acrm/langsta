using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangStat.Contracts
{
    public class LanguageStatisticsBuildingResult
    {
        public LanguageStatisticsBuildingResult()
        {
            BuildIsSuccessful = true;
        }

        public bool BuildIsSuccessful { get; set; }

        public string ErrorMessage { get; set; }

        public LanguageStatistics Result { get; set; }
    }
}
