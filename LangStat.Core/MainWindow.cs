using LangStat.Contracts;
using LangStat.DataAccess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LangStat.Core
{
    public class MainWindow
    {
        private readonly WordsRepository wordsRepository = new WordsRepository();

        public ILanguagesRepository LanguagesRepository { get; private set; }

        public MainWindow()
        {
            var languagesDao = new LanguagesDao();
            LanguagesRepository = new LanguagesRepository(languagesDao);
        }
    }
}
