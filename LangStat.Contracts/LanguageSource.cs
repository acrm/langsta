using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LangStat.Contracts
{
    public class LanguageSource
    {
        private readonly Language _language;
        //private readonly WordsRepository _wordsRepository;

        public LanguageSource(Language language)
        {
            _language = language;
            //_wordsRepository = language.WordsRepository;
        }

        public Guid Id { get; set; }

        public string Address { get; set; }

        public void AcquireWords()
        {
 
        }

        public double WordsAcquirementProgress { get; set; }

        public string Content { get; set; }
    }
}
