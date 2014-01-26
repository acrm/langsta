using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangStat.Contracts
{
    public class LanguageSourceCreationResponse
    {
        public bool IsSuccessful { get; set; }

        public Guid Id { get; set; }
    }
}
