using LangStat.Core.Contracts;
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
        public ILanguagesRepository LanguagesRepository { get; private set; }

        public StatisticsProcessor StatisticsProcessor { get; private set; }

        public MainWindow()
        {
            var languagesDao = new LanguagesDao();
            var languagesSourcesDao = new LanguagesSourcesDao(languagesDao);
            LanguagesRepository = new LanguagesRepository(languagesDao, languagesSourcesDao);
            StatisticsProcessor = new StatisticsProcessor(LanguagesRepository);
        }
    }
}
