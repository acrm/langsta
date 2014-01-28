using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LangStat.Core.Contracts
{
    public class LanguageSource
    {
        public LanguageSource(string languageName)
        {
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
