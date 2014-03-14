using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangStat.Contracts
{
    public struct WordStatistics
    {
        public string Spelling { get; set; }

        public int CountOfAccurances { get; set; }
    }
}
