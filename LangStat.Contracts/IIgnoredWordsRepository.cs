using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangStat.Core.Contracts
{
    public interface IIgnoredWordsRepository
    {
        string[] GetAllWords();

        bool AddWord(string word);
    }
}
